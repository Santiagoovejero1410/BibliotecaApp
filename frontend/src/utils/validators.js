// Validaciones simples del lado del cliente (el backend VUELVE a validar: nunca confiar solo en el front).

export const esEmailValido = (email) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
