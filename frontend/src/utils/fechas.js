// Funciones auxiliares para trabajar con fechas (JavaScript no trae muchas cosas listas).

export const HORA_INICIO = 8;   // La agenda arranca a las 8:00
export const HORA_FIN = 20;     // y termina a las 20:00
export const HORA_PX = 60;      // Alto en píxeles de cada hora en la agenda

export const pad2 = (n) => String(n).padStart(2, "0");   // 5 -> "05"

// Devuelve el LUNES de la semana de una fecha
export function inicioDeSemana(fecha) {
  const d = new Date(fecha);
  d.setHours(0, 0, 0, 0);
  const dia = d.getDay();                  // 0 = domingo ... 6 = sábado
  d.setDate(d.getDate() + (dia === 0 ? -6 : 1 - dia));
  return d;
}

export function sumarDias(fecha, n) {
  const d = new Date(fecha);
  d.setDate(d.getDate() + n);
  return d;
}

// Fecha -> "2024-08-05T09:00:00" (hora LOCAL, sin zona horaria). Es el formato que espera el backend.
export function aISOLocal(f) {
  return `${f.getFullYear()}-${pad2(f.getMonth() + 1)}-${pad2(f.getDate())}T${pad2(f.getHours())}:${pad2(f.getMinutes())}:00`;
}

// Fecha -> "2024-08-05" (formato del <input type="date">)
export function aFechaInput(f) {
  return `${f.getFullYear()}-${pad2(f.getMonth() + 1)}-${pad2(f.getDate())}`;
}

export function formatearHora(f) {
  return `${pad2(f.getHours())}:${pad2(f.getMinutes())}`;
}

export function formatearFechaHora(iso) {
  return new Date(iso).toLocaleString("es-AR", { dateStyle: "medium", timeStyle: "short" });
}

export function mismoDia(a, b) {
  return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate();
}
