import {
  HORA_INICIO, HORA_FIN, HORA_PX,
  sumarDias, mismoDia, formatearHora, pad2,
} from "../utils/fechas.js";

const DIAS = ["Lun", "Mar", "Mié", "Jue", "Vie", "Sáb"];
const CANT_HORAS = HORA_FIN - HORA_INICIO;

// Agenda semanal (Lunes a Sábado) con los turnos dibujados como bloques.
// Props:
//  - inicioSemana: fecha del lunes que se está mostrando
//  - turnos: lista que viene de la API
//  - seleccionado: id del turno elegido (para resaltarlo)
//  - onSeleccionar(turno): click en un turno
//  - onClickHueco(dia, hora, minutos): click en un espacio vacío (para crear turno)
export default function WeeklyCalendar({ inicioSemana, turnos, seleccionado, onSeleccionar, onClickHueco }) {
  // Armamos los 6 días de la semana a partir del lunes
  const dias = DIAS.map((nombre, i) => ({ nombre, fecha: sumarDias(inicioSemana, i) }));
  const hoy = new Date();

  // Un array con las horas para dibujar la columna de la izquierda (8, 9, 10...)
  const horas = Array.from({ length: CANT_HORAS }, (_, i) => HORA_INICIO + i);

  // Convierte el click en el espacio vacío en una hora (redondeada a bloques de 30 min)
  const alClickColumna = (e, fecha) => {
    const rect = e.currentTarget.getBoundingClientRect();
    const y = e.clientY - rect.top;
    const minutosTotales = Math.floor(y / HORA_PX * 2) * 30;
    onClickHueco(fecha, HORA_INICIO + Math.floor(minutosTotales / 60), minutosTotales % 60);
  };

  // Estilos de posición de un bloque: "top" según la hora de inicio y "height" según la duración
  const estiloBloque = (t) => {
    const inicio = new Date(t.fechaHora);
    const minDesdeApertura = (inicio.getHours() - HORA_INICIO) * 60 + inicio.getMinutes();
    return {
      top: (minDesdeApertura / 60) * HORA_PX,
      height: Math.max((t.duracionMin / 60) * HORA_PX - 2, 22),
    };
  };

  return (
    <div className="calendario">
      {/* Fila de encabezado con los días */}
      <div className="cal-header">
        <div className="cal-gmt" />
        {dias.map((d) => (
          <div key={d.nombre} className={`cal-dia ${mismoDia(d.fecha, hoy) ? "hoy" : ""}`}>
            <span className="cal-dia-nombre">{d.nombre}</span>
            <span className="cal-dia-num">{d.fecha.getDate()}</span>
          </div>
        ))}
      </div>

      <div className="cal-body">
        {/* Columna de horas */}
        <div className="cal-horas">
          {horas.map((h) => (
            <div key={h} className="cal-hora">{pad2(h)}:00</div>
          ))}
        </div>

        {/* Una columna por día */}
        {dias.map((d) => (
          <div key={d.nombre} className="cal-col" onClick={(e) => alClickColumna(e, d.fecha)}>
            {turnos
              // Solo los turnos de este día y que no estén cancelados
              .filter((t) => mismoDia(new Date(t.fechaHora), d.fecha) && t.estado !== "Cancelado")
              .map((t) => (
                <div
                  key={t.id}
                  className={`turno-bloque estado-${t.estado.toLowerCase()} ${seleccionado === t.id ? "seleccionado" : ""}`}
                  style={estiloBloque(t)}
                  onClick={(e) => {
                    e.stopPropagation();       // Evita que el click también cuente como "click en hueco"
                    onSeleccionar(t);
                  }}
                >
                  <strong>{t.servicioNombre}</strong>
                  <span>
                    {formatearHora(new Date(t.fechaHora))} - {formatearHora(new Date(t.fechaFin))}
                  </span>
                </div>
              ))}
          </div>
        ))}
      </div>
    </div>
  );
}
