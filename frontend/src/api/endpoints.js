import api from "./axios.js";

// Todas las llamadas al backend, agrupadas por recurso.
// Las páginas usan estas funciones y no se preocupan por URLs ni métodos HTTP.

export const authApi = {
  login: (datos) => api.post("/auth/login", datos).then((r) => r.data),
  register: (datos) => api.post("/auth/register", datos).then((r) => r.data),
};

export const serviciosApi = {
  listar: () => api.get("/servicios").then((r) => r.data),
  crear: (datos) => api.post("/servicios", datos).then((r) => r.data),
  actualizar: (id, datos) => api.put(`/servicios/${id}`, datos).then((r) => r.data),
  eliminar: (id) => api.delete(`/servicios/${id}`),
};

export const turnosApi = {
  // params opcional: { desde, hasta } para filtrar por rango de fechas
  listar: (params) => api.get("/turnos", { params }).then((r) => r.data),
  crear: (datos) => api.post("/turnos", datos).then((r) => r.data),
  cambiarEstado: (id, estado) => api.put(`/turnos/${id}/estado`, { estado }).then((r) => r.data),
  eliminar: (id) => api.delete(`/turnos/${id}`),
};
