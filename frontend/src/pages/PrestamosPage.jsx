import { useEffect, useState } from "react";
import api, { getError } from "../api/api.js";
import { useAuth } from "../context/AuthContext.jsx";

// Lista de préstamos: el socio ve los suyos, el admin ve todos (lo decide el backend según el token).
export default function PrestamosPage() {
  const { isAdmin } = useAuth();

  const [prestamos, setPrestamos] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState("");

  const cargar = async () => {
    setCargando(true);
    setError("");
    try {
      const res = await api.get("/prestamos");
      setPrestamos(res.data);
    } catch (err) {
      setError(getError(err));
    } finally {
      setCargando(false);
    }
  };

  useEffect(() => {
    cargar();
  }, []);

  const devolver = async (prestamo) => {
    setError("");
    try {
      await api.put(`/prestamos/${prestamo.id}/devolver`);
      cargar();
    } catch (err) {
      setError(getError(err));
    }
  };

  const formatear = (fecha) => new Date(fecha).toLocaleDateString("es-AR");

  return (
    <main className="contenido">
      <div className="page-heading">
        <div>
          <div className="eyebrow">Seguimiento de lecturas</div>
          <h1>{isAdmin ? "Todos los préstamos" : "Mis préstamos"}</h1>
          <p>{isAdmin ? "Consultá los préstamos registrados en la biblioteca." : "Revisá tus libros y fechas de devolución."}</p>
        </div>
      </div>

      {error && <p className="alerta">{error}</p>}
      {cargando && <p className="muted">Cargando préstamos...</p>}
      {!cargando && prestamos.length === 0 && !error && <div className="panel empty-state"><span>▤</span>Todavía no hay préstamos para mostrar.</div>}

      {prestamos.length > 0 && (
        <section className="panel">
          <div className="table-scroll">
            <table>
              <thead><tr>
                <th>Libro</th>
                {isAdmin && <th>Socio</th>}
                <th>Fecha de préstamo</th>
                <th>Devolución</th>
                <th>Acciones</th>
              </tr></thead>
              <tbody>
                {prestamos.map((p) => (
                  <tr key={p.id}>
                    <td><strong>{p.libroTitulo}</strong></td>
                    {isAdmin && <td>{p.usuarioNombre}</td>}
                    <td>{formatear(p.fechaPrestamo)}</td>
                    <td>{p.devuelto ? formatear(p.fechaDevolucion) : <span className="badge rojo">● Pendiente</span>}</td>
                    <td>{!p.devuelto && <button className="btn btn-primary" onClick={() => devolver(p)}>Marcar devuelto</button>}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>
      )}
    </main>
  );
}
