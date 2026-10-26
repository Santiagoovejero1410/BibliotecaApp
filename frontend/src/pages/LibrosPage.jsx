import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import api, { getError } from "../api/api.js";
import { useAuth } from "../context/AuthContext.jsx";

// Listado de libros con búsqueda y paginación (los datos vienen del backend).
export default function LibrosPage() {
  const { isAdmin } = useAuth();

  const [data, setData] = useState({ items: [], total: 0, pagina: 1, totalPaginas: 1 });
  const [pagina, setPagina] = useState(1);
  const [texto, setTexto] = useState("");     // lo que se escribe en el buscador
  const [buscar, setBuscar] = useState("");   // lo que se envió a buscar
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState("");
  const [mensaje, setMensaje] = useState("");

  // Pide los libros a la API
  const cargar = async () => {
    setCargando(true);
    setError("");
    try {
      const res = await api.get("/libros", { params: { pagina, tamanio: 5, buscar } });
      setData(res.data);
    } catch (err) {
      setError(getError(err));
    } finally {
      setCargando(false);
    }
  };

  // useEffect: se ejecuta al abrir la página y cada vez que cambia la página o la búsqueda
  useEffect(() => {
    cargar();
  }, [pagina, buscar]);

  const handleBuscar = (e) => {
    e.preventDefault();
    setPagina(1);          // al buscar, volvemos a la primera página
    setBuscar(texto);
  };

  const pedirPrestado = async (libro) => {
    setMensaje("");
    setError("");
    try {
      await api.post("/prestamos", { libroId: libro.id });
      setMensaje(`Pediste prestado "${libro.titulo}".`);
      cargar();
    } catch (err) {
      setError(getError(err));
    }
  };

  const eliminar = async (libro) => {
    if (!window.confirm(`¿Eliminar "${libro.titulo}"?`)) return;
    setMensaje("");
    setError("");
    try {
      await api.delete(`/libros/${libro.id}`);
      cargar();
    } catch (err) {
      setError(getError(err));
    }
  };

  return (
    <main className="contenido">
      <div className="page-header">
        <h2>Libros</h2>
        {/* Este botón solo se muestra si el usuario es Admin */}
        {isAdmin && <Link to="/libros/nuevo" className="btn btn-primary">+ Nuevo libro</Link>}
      </div>

      <form className="buscador" onSubmit={handleBuscar}>
        <input placeholder="Buscar por título o autor..." value={texto} onChange={(e) => setTexto(e.target.value)} />
        <button className="btn btn-outline">Buscar</button>
      </form>

      {mensaje && <p className="ok">{mensaje}</p>}
      {error && <p className="alerta">{error}</p>}
      {cargando && <p className="muted">Cargando libros...</p>}

      {!cargando && data.items.length === 0 && !error && <p className="muted">No se encontraron libros.</p>}

      {data.items.length > 0 && (
        <table>
          <thead>
            <tr>
              <th>Título</th>
              <th>Autor</th>
              <th>Género</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {/* key: React necesita un identificador único por cada elemento de la lista */}
            {data.items.map((libro) => (
              <tr key={libro.id}>
                <td>{libro.titulo}</td>
                <td>{libro.autor}</td>
                <td>{libro.genero}</td>
                <td>
                  <span className={libro.disponible ? "badge verde" : "badge rojo"}>
                    {libro.disponible ? "Disponible" : "Prestado"}
                  </span>
                </td>
                <td className="acciones">
                  <button className="btn btn-primary" disabled={!libro.disponible} onClick={() => pedirPrestado(libro)}>
                    Pedir prestado
                  </button>
                  {isAdmin && (
                    <>
                      <Link to={`/libros/${libro.id}/editar`} className="btn btn-outline">Editar</Link>
                      <button className="btn btn-danger" onClick={() => eliminar(libro)}>Eliminar</button>
                    </>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {/* Paginación */}
      <div className="paginacion">
        <button className="btn btn-outline" disabled={pagina <= 1} onClick={() => setPagina(pagina - 1)}>Anterior</button>
        <span>Página {data.pagina} de {data.totalPaginas}</span>
        <button className="btn btn-outline" disabled={pagina >= data.totalPaginas} onClick={() => setPagina(pagina + 1)}>Siguiente</button>
      </div>
    </main>
  );
}
