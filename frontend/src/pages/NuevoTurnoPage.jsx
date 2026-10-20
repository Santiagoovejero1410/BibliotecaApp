import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { serviciosApi, turnosApi } from "../api/endpoints.js";
import { mensajeError } from "../api/axios.js";
import { Cargando, ErrorMsg } from "../components/Estado.jsx";
import { HORA_INICIO, HORA_FIN, aFechaInput, pad2 } from "../utils/fechas.js";

// Horarios disponibles cada 30 minutos: 08:00, 08:30, ... 19:30
const HORARIOS = [];
for (let h = HORA_INICIO; h < HORA_FIN; h++) {
  HORARIOS.push(`${pad2(h)}:00`, `${pad2(h)}:30`);
}

export default function NuevoTurnoPage() {
  const navigate = useNavigate();
  // Si venimos de hacer click en la agenda, la URL trae ?fecha=...&hora=... y precargamos el formulario
  const [params] = useSearchParams();

  const [servicios, setServicios] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [errorCarga, setErrorCarga] = useState("");

  const [form, setForm] = useState({
    servicioId: "",
    fecha: params.get("fecha") || "",
    hora: params.get("hora") || "",
    notas: "",
  });
  const [errores, setErrores] = useState({});
  const [errorApi, setErrorApi] = useState("");
  const [enviando, setEnviando] = useState(false);

  // Al abrir la página, traemos los servicios para llenar el <select>
  useEffect(() => {
    const cargar = async () => {
      try {
        const data = await serviciosApi.listar();
        setServicios(data.filter((s) => s.activo));
      } catch (err) {
        setErrorCarga(mensajeError(err));
      } finally {
        setCargando(false);
      }
    };
    cargar();
  }, []);

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  const validar = () => {
    const e = {};
    if (!form.servicioId) e.servicioId = "Elegí un servicio";
    if (!form.fecha) e.fecha = "Elegí una fecha";
    if (!form.hora) e.hora = "Elegí un horario";
    if (form.fecha && form.hora && new Date(`${form.fecha}T${form.hora}:00`) <= new Date()) {
      e.fecha = "La fecha y hora deben ser futuras";
    }
    if (form.notas.length > 300) e.notas = "Máximo 300 caracteres";
    return e;
  };

  const handleSubmit = async (ev) => {
    ev.preventDefault();
    const e = validar();
    setErrores(e);
    if (Object.keys(e).length > 0) return;

    setEnviando(true);
    setErrorApi("");
    try {
      await turnosApi.crear({
        servicioId: Number(form.servicioId),            // El <select> devuelve texto: lo pasamos a número
        fechaHora: `${form.fecha}T${form.hora}:00`,
        notas: form.notas.trim() || null,
      });
      navigate("/");                                    // Éxito: volvemos a la agenda
    } catch (err) {
      setErrorApi(mensajeError(err));                   // Ej: "Ese horario ya está ocupado"
    } finally {
      setEnviando(false);
    }
  };

  if (cargando) return <Cargando />;

  return (
    <div>
      <div className="page-header"><h1>Nuevo turno</h1></div>
      <ErrorMsg mensaje={errorCarga} />

      <form className="card form-card" onSubmit={handleSubmit} noValidate>
        <ErrorMsg mensaje={errorApi} />

        <label>Servicio</label>
        <select name="servicioId" value={form.servicioId} onChange={handleChange}>
          <option value="">-- Elegí un servicio --</option>
          {servicios.map((s) => (
            <option key={s.id} value={s.id}>
              {s.nombre} ({s.duracionMin} min - ${s.precio})
            </option>
          ))}
        </select>
        {errores.servicioId && <p className="error-texto">{errores.servicioId}</p>}

        <div className="fila-2">
          <div>
            <label>Fecha</label>
            <input name="fecha" type="date" min={aFechaInput(new Date())} value={form.fecha} onChange={handleChange} />
            {errores.fecha && <p className="error-texto">{errores.fecha}</p>}
          </div>
          <div>
            <label>Horario</label>
            <select name="hora" value={form.hora} onChange={handleChange}>
              <option value="">--:--</option>
              {HORARIOS.map((h) => <option key={h} value={h}>{h}</option>)}
            </select>
            {errores.hora && <p className="error-texto">{errores.hora}</p>}
          </div>
        </div>

        <label>Notas (opcional)</label>
        <textarea name="notas" rows="3" value={form.notas} onChange={handleChange} />
        {errores.notas && <p className="error-texto">{errores.notas}</p>}

        <div className="form-botones">
          <button type="button" className="btn btn-outline" onClick={() => navigate(-1)}>Volver</button>
          <button className="btn btn-primary" disabled={enviando}>
            {enviando ? "Reservando..." : "Reservar turno"}
          </button>
        </div>
      </form>
    </div>
  );
}
