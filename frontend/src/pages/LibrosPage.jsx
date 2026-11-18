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
      <div className="page-heading">
        <div>
          <div className="eyebrow">Explorá nuestra colección</div>
          <h1>Catálogo de libros</h1>
          <p>Encontrá tu próxima lectura entre nuestros títulos.</p>
        </div>
        {isAdmin && <Link to="/libros/nuevo" className="btn btn-primary">＋ Agregar libro</Link>}
      </div>

      <div className="toolbar">
        <form className="buscador" onSubmit={handleBuscar}>
          <div className="search-wrap"><span className="search-symbol">⌕</span>
            <input aria-label="Buscar libros" placeholder="Buscar por título o autor..." value={texto} onChange={(e) => setTexto(e.target.value)} />
          </div>
          <button className="btn btn-outline">Buscar</button>
        </form>
        <span className="muted small">{data.total ?? data.items.length} títulos en el catálogo</span>
      </div>

      {mensaje && <p className="ok">{mensaje}</p>}
      {error && <p className="alerta">{error}</p>}
      {cargando && <p className="muted">Cargando libros...</p>}
      {!cargando && data.items.length === 0 && !error && <div className="panel empty-state"><span>⌕</span>No se encontraron libros. Probá con otra búsqueda.</div>}

      {data.items.length > 0 && (
        <section className="catalog-grid">
          {data.items.map((libro) => (
            <article className="book-card" key={libro.id}>
              <div className="book-cover" aria-hidden="true">▤</div>
              <div className="book-info">
                <div className="book-genre">{libro.genero || "Lectura"}</div>
                <h3>{libro.titulo}</h3>
                <div className="book-author">por {libro.autor}</div>
                <span className={libro.disponible ? "badge verde" : "badge rojo"}>
                  {libro.disponible ? "● Disponible" : "● Prestado"}
                </span>
                <div className="book-actions">
                  <button className="btn btn-primary" disabled={!libro.disponible} onClick={() => pedirPrestado(libro)}>
                    Pedir prestado
                  </button>
                  {isAdmin && <>
                    <Link to={`/libros/${libro.id}/editar`} className="btn btn-outline">Editar</Link>
                    <button className="btn btn-danger" onClick={() => eliminar(libro)}>Eliminar</button>
                  </>}
                </div>
              </div>
            </article>
          ))}
        </section>
      )}

      <div className="paginacion">
        <button className="btn btn-outline" disabled={pagina <= 1} onClick={() => setPagina(pagina - 1)}>← Anterior</button>
        <span>Página {data.pagina} de {data.totalPaginas}</span>
        <button className="btn btn-outline" disabled={pagina >= data.totalPaginas} onClick={() => setPagina(pagina + 1)}>Siguiente →</button>
      </div>
    </main>
  );
}
