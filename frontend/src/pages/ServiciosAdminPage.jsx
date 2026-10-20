import { useCallback, useEffect, useState } from "react";
import { serviciosApi } from "../api/endpoints.js";
import { mensajeError } from "../api/axios.js";
import { Cargando, ErrorMsg } from "../components/Estado.jsx";

const FORM_VACIO = { nombre: "", duracionMin: "", precio: "", activo: true };

// CRUD completo de servicios (solo Admin):
//  C = crear (formulario), R = listar (tabla), U = editar (mismo formulario), D = eliminar.
export default function ServiciosAdminPage() {
  const [servicios, setServicios] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState("");

  const [form, setForm] = useState(FORM_VACIO);
  const [editandoId, setEditandoId] = useState(null);   // null = estamos creando; un id = estamos editando
  const [errores, setErrores] = useState({});
  const [errorForm, setErrorForm] = useState("");
  const [guardando, setGuardando] = useState(false);

  const cargar = useCallback(async () => {
    setCargando(true);
    setError("");
    try {
      setServicios(await serviciosApi.listar());
    } catch (err) {
      setError(mensajeError(err));
    } finally {
      setCargando(false);
    }
  }, []);

  useEffect(() => {
    cargar();
  }, [cargar]);

  // Un solo handler: los checkbox usan "checked" y el resto "value"
  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === "checkbox" ? checked : value });
  };

  const validar = () => {
    const e = {};
    if (form.nombre.trim().length < 2) e.nombre = "El nombre debe tener al menos 2 caracteres";
    const dur = Number(form.duracionMin);
    if (!form.duracionMin || dur < 5 || dur > 480) e.duracionMin = "La duración debe estar entre 5 y 480 minutos";
    if (form.precio === "" || Number(form.precio) < 0) e.precio = "El precio debe ser mayor o igual a 0";
    return e;
  };

  const handleSubmit = async (ev) => {
    ev.preventDefault();
    const e = validar();
    setErrores(e);
    if (Object.keys(e).length > 0) return;

    const datos = {
      nombre: form.nombre.trim(),
      duracionMin: Number(form.duracionMin),
      precio: Number(form.precio),
      activo: form.activo,
    };

    setGuardando(true);
    setErrorForm("");
    try {
      if (editandoId) await serviciosApi.actualizar(editandoId, datos);   // PUT
      else await serviciosApi.crear(datos);                                // POST
      cancelarEdicion();
      cargar();
    } catch (err) {
      setErrorForm(mensajeError(err));
    } finally {
      setGuardando(false);
    }
  };

  // Carga los datos del servicio en el formulario para editarlo
  const editar = (s) => {
    setEditandoId(s.id);
    setForm({ nombre: s.nombre, duracionMin: s.duracionMin, precio: s.precio, activo: s.activo });
    setErrores({});
    setErrorForm("");
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const cancelarEdicion = () => {
    setEditandoId(null);
    setForm(FORM_VACIO);
    setErrores({});
  };

  const eliminar = async (s) => {
    if (!window.confirm(`¿Eliminar el servicio "${s.nombre}"?`)) return;
    try {
      await serviciosApi.eliminar(s.id);
      cargar();
    } catch (err) {
      setError(mensajeError(err));    // Ej: "No se puede eliminar: tiene turnos asociados"
    }
  };

  return (
    <div>
      <div className="page-header"><h1>Servicios</h1></div>

      <form className="card form-card" onSubmit={handleSubmit} noValidate>
        <h3>{editandoId ? "Editar servicio" : "Nuevo servicio"}</h3>
        <ErrorMsg mensaje={errorForm} />

        <label>Nombre</label>
        <input name="nombre" value={form.nombre} onChange={handleChange} />
        {errores.nombre && <p className="error-texto">{errores.nombre}</p>}

        <div className="fila-2">
          <div>
            <label>Duración (minutos)</label>
            <input name="duracionMin" type="number" value={form.duracionMin} onChange={handleChange} />
            {errores.duracionMin && <p className="error-texto">{errores.duracionMin}</p>}
          </div>
          <div>
            <label>Precio ($)</label>
            <input name="precio" type="number" value={form.precio} onChange={handleChange} />
            {errores.precio && <p className="error-texto">{errores.precio}</p>}
          </div>
        </div>

        <label className="check">
          <input name="activo" type="checkbox" checked={form.activo} onChange={handleChange} /> Activo (se puede reservar)
        </label>

        <div className="form-botones">
          {editandoId && <button type="button" className="btn btn-outline" onClick={cancelarEdicion}>Cancelar</button>}
          <button className="btn btn-primary" disabled={guardando}>
            {guardando ? "Guardando..." : editandoId ? "Guardar cambios" : "Crear servicio"}
          </button>
        </div>
      </form>

      <ErrorMsg mensaje={error} onReintentar={cargar} />
      {cargando && <Cargando />}

      {!cargando && servicios.length > 0 && (
        <div className="card tabla-wrap">
          <table>
            <thead>
              <tr><th>Nombre</th><th>Duración</th><th>Precio</th><th>Estado</th><th>Acciones</th></tr>
            </thead>
            <tbody>
              {servicios.map((s) => (
                <tr key={s.id}>
                  <td>{s.nombre}</td>
                  <td>{s.duracionMin} min</td>
                  <td>${s.precio}</td>
                  <td>
                    <span className={`badge ${s.activo ? "estado-confirmado" : "estado-cancelado"}`}>
                      {s.activo ? "Activo" : "Inactivo"}
                    </span>
                  </td>
                  <td className="acciones">
                    <button className="btn btn-small btn-outline" onClick={() => editar(s)}>Editar</button>
                    <button className="btn btn-small btn-danger" onClick={() => eliminar(s)}>Eliminar</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
