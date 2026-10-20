import { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { turnosApi } from "../api/endpoints.js";
import { mensajeError } from "../api/axios.js";
import WeeklyCalendar from "../components/WeeklyCalendar.jsx";
import TurnoAcciones from "../components/TurnoAcciones.jsx";
import { Cargando, ErrorMsg } from "../components/Estado.jsx";
import {
  inicioDeSemana, sumarDias, aISOLocal, aFechaInput,
  formatearHora, formatearFechaHora, pad2,
} from "../utils/fechas.js";

// Pantalla principal: agenda semanal (como la del diseño de referencia).
export default function AgendaPage() {
  const navigate = useNavigate();

  const [inicioSemana, setInicioSemana] = useState(() => inicioDeSemana(new Date()));
  const [turnos, setTurnos] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState("");
  const [seleccionado, setSeleccionado] = useState(null);   // turno cuyo detalle se muestra a la derecha

  // useCallback memoriza la función; solo cambia cuando cambia la semana mostrada
  const cargar = useCallback(async () => {
    setCargando(true);
    setError("");
    try {
      // Pedimos solo los turnos de la semana visible (filtro desde/hasta en el backend)
      const data = await turnosApi.listar({
        desde: aISOLocal(inicioSemana),
        hasta: aISOLocal(sumarDias(inicioSemana, 7)),
      });
      setTurnos(data);
    } catch (err) {
      setError(mensajeError(err));
    } finally {
      setCargando(false);
    }
  }, [inicioSemana]);

  // useEffect: se ejecuta al montar la página y cada vez que cambia "cargar" (o sea, la semana)
  useEffect(() => {
    cargar();
  }, [cargar]);

  const cambiarSemana = (dias) => {
    setSeleccionado(null);
    setInicioSemana(sumarDias(inicioSemana, dias));
  };

  // Click en un hueco vacío -> va al formulario con fecha y hora ya cargadas
  const crearEnHueco = (fecha, hora, min) => {
    navigate(`/turnos/nuevo?fecha=${aFechaInput(fecha)}&hora=${pad2(hora)}:${pad2(min)}`);
  };

  // Después de confirmar/cancelar/eliminar: cerramos el panel y recargamos
  const despuesDeAccion = () => {
    setSeleccionado(null);
    cargar();
  };

  const tituloMes = inicioSemana.toLocaleDateString("es-AR", { month: "long", year: "numeric" });

  return (
    <div>
      <div className="page-header">
        <h1>Agenda</h1>
        <button className="btn btn-primary" onClick={() => navigate("/turnos/nuevo")}>+ Nuevo turno</button>
      </div>

      <div className="toolbar">
        <div className="semana-nav">
          <button className="btn btn-outline btn-small" onClick={() => cambiarSemana(-7)}>‹</button>
          <button className="btn btn-outline btn-small" onClick={() => setInicioSemana(inicioDeSemana(new Date()))}>Hoy</button>
          <button className="btn btn-outline btn-small" onClick={() => cambiarSemana(7)}>›</button>
          <strong className="mes-titulo">{tituloMes}</strong>
        </div>
        <div className="leyenda">
          <span><i className="punto estado-pendiente" /> Pendiente</span>
          <span><i className="punto estado-confirmado" /> Confirmado</span>
        </div>
      </div>

      <ErrorMsg mensaje={error} onReintentar={cargar} />

      <div className={`agenda-layout ${seleccionado ? "con-panel" : ""}`}>
        <div className="calendario-wrap">
          {cargando && <div className="overlay-carga"><Cargando /></div>}
          <WeeklyCalendar
            inicioSemana={inicioSemana}
            turnos={turnos}
            seleccionado={seleccionado?.id}
            onSeleccionar={setSeleccionado}
            onClickHueco={crearEnHueco}
          />
        </div>

        {/* Panel de detalle (como el "Edit Schedule" del diseño) */}
        {seleccionado && (
          <aside className="card panel-detalle">
            <div className="panel-titulo">
              <h3>Detalle del turno</h3>
              <button className="btn-cerrar" onClick={() => setSeleccionado(null)}>✕</button>
            </div>
            <p><strong>{seleccionado.servicioNombre}</strong></p>
            <p className="muted">👤 {seleccionado.usuarioNombre}</p>
            <p className="muted">🕒 {formatearFechaHora(seleccionado.fechaHora)} → {formatearHora(new Date(seleccionado.fechaFin))}</p>
            <p><span className={`badge estado-${seleccionado.estado.toLowerCase()}`}>{seleccionado.estado}</span></p>
            {seleccionado.notas && <p className="muted">📝 {seleccionado.notas}</p>}
            <TurnoAcciones turno={seleccionado} onChange={despuesDeAccion} />
          </aside>
        )}
      </div>
    </div>
  );
}
