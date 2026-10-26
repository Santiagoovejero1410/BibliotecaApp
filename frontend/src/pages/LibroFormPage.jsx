import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import api, { getError } from "../api/api.js";

// Formulario para crear o editar un libro (solo Admin).
// Si la URL trae un :id (/libros/3/editar) estamos EDITANDO; si no, estamos CREANDO.
export default function LibroFormPage() {
  const { id } = useParams();            // lee el :id de la URL
  const navigate = useNavigate();
  const editando = Boolean(id);

  const [form, setForm] = useState({ titulo: "", autor: "", genero: "" });
  const [errores, setErrores] = useState({});
  const [errorApi, setErrorApi] = useState("");
  const [cargando, setCargando] = useState(editando);   // si editamos, primero hay que cargar el libro
  const [guardando, setGuardando] = useState(false);

  // Si estamos editando, traemos los datos del libro para completar el formulario
  useEffect(() => {
    if (!editando) return;
    const cargar = async () => {
      try {
        const { data } = await api.get(`/libros/${id}`);
        setForm({ titulo: data.titulo, autor: data.autor, genero: data.genero });
      } catch (err) {
        setErrorApi(getError(err));
      } finally {
        setCargando(false);
      }
    };
    cargar();
  }, [id, editando]);

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  const validar = () => {
    const e = {};
    if (form.titulo.trim().length < 2) e.titulo = "El título debe tener al menos 2 caracteres";
    if (form.autor.trim().length < 2) e.autor = "El autor debe tener al menos 2 caracteres";
    if (form.genero.length > 50) e.genero = "Máximo 50 caracteres";
    return e;
  };

  const handleSubmit = async (ev) => {
    ev.preventDefault();
    const e = validar();
    setErrores(e);
    if (Object.keys(e).length > 0) return;

    setGuardando(true);
    setErrorApi("");
    try {
      if (editando) await api.put(`/libros/${id}`, form);   // PUT = actualizar
      else await api.post("/libros", form);                  // POST = crear
      navigate("/");
    } catch (err) {
      setErrorApi(getError(err));
    } finally {
      setGuardando(false);
    }
  };

  if (cargando) return <main className="contenido"><p className="muted">Cargando...</p></main>;

  return (
    <main className="contenido">
      <h2>{editando ? "Editar libro" : "Nuevo libro"}</h2>

      <form className="card form-card" onSubmit={handleSubmit} noValidate>
        {errorApi && <p className="alerta">{errorApi}</p>}

        <label>Título</label>
        <input name="titulo" value={form.titulo} onChange={handleChange} />
        {errores.titulo && <p className="error-texto">{errores.titulo}</p>}

        <label>Autor</label>
        <input name="autor" value={form.autor} onChange={handleChange} />
        {errores.autor && <p className="error-texto">{errores.autor}</p>}

        <label>Género</label>
        <input name="genero" value={form.genero} onChange={handleChange} />
        {errores.genero && <p className="error-texto">{errores.genero}</p>}

        <div className="acciones">
          <button type="button" className="btn btn-outline" onClick={() => navigate("/")}>Cancelar</button>
          <button className="btn btn-primary" disabled={guardando}>
            {guardando ? "Guardando..." : "Guardar"}
          </button>
        </div>
      </form>
    </main>
  );
}
