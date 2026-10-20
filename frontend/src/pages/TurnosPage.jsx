import { useCallback, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { turnosApi } from "../api/endpoints.js";
import { mensajeError } from "../api/axios.js";
import { useAuth } from "../context/AuthContext.jsx";
import TurnoAcciones from "../components/TurnoAcciones.jsx";
import { Cargando, ErrorMsg } from "../components/Estado.jsx";
import { formatearFechaHora } from "../utils/fechas.js";

// Listado en tabla de turnos (el cliente ve los suyos, el admin ve todos) con filtro y búsqueda.
export default function TurnosPage() {
  const { isAdmin } = useAuth();

  const [turnos, setTurnos] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState("");
  const [filtroEstado, setFiltroEstado] = useState("Todos");
  const [busqueda, setBusqueda] = useState("");

  const cargar = useCallback(async () => {
    setCargando(true);
    setError("");
    try {
      setTurnos(await turnosApi.listar());
    } catch (err) {
      setError(mensajeError(err));
    } finally {
      setCargando(false);
    }
  }, []);

  useEffect(() => {
    cargar();
  }, [cargar]);

  // Filtrado en el cliente: por estado y por texto (servicio o nombre de la persona)
  const visibles = turnos.filter((t) => {
    const coincideEstado = filtroEstado === "Todos" || t.estado === filtroEstado;
    const texto = busqueda.trim().toLowerCase();
    const coincideTexto =
      !texto ||
      t.servicioNombre.toLowerCase().includes(texto) ||
      t.usuarioNombre.toLowerCase().includes(texto);
    return coincideEstado && coincideTexto;
  });

  return (
    <div>
      <div className="page-header">
        <h1>{isAdmin ? "Todos los turnos" : "Mis turnos"}</h1>
        <Link to="/turnos/nuevo" className="btn btn-primary">+ Nuevo turno</Link>
      </div>

      <div className="card filtros">
        <input
          placeholder="Buscar por servicio o persona..."
          value={busqueda}
          onChange={(e) => setBusqueda(e.target.value)}
        />
        <select value={filtroEstado} onChange={(e) => setFiltroEstado(e.target.value)}>
          <option>Todos</option>
          <option>Pendiente</option>
          <option>Confirmado</option>
          <option>Cancelado</option>
        </select>
      </div>

      <ErrorMsg mensaje={error} onReintentar={cargar} />
      {cargando && <Cargando texto="Cargando turnos..." />}

      {!cargando && !error && visibles.length === 0 && (
        <div className="card vacio">No hay turnos para mostrar.</div>
      )}

      {!cargando && visibles.length > 0 && (
        <div className="card tabla-wrap">
          <table>
            <thead>
              <tr>
                <th>Fecha y hora</th>
                <th>Servicio</th>
                {isAdmin && <th>Persona</th>}
                <th>Estado</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {/* key: React necesita un identificador único por elemento de la lista para actualizarla bien */}
              {visibles.map((t) => (
                <tr key={t.id}>
                  <td>{formatearFechaHora(t.fechaHora)}</td>
                  <td>{t.servicioNombre}</td>
                  {isAdmin && <td>{t.usuarioNombre}</td>}
                  <td><span className={`badge estado-${t.estado.toLowerCase()}`}>{t.estado}</span></td>
                  <td><TurnoAcciones turno={t} onChange={cargar} /></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
