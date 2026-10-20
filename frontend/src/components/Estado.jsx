// Componentes chicos y reutilizables para los estados de "cargando" y "error".

export function Cargando({ texto = "Cargando..." }) {
  return (
    <div className="estado">
      <div className="spinner" />
      <p>{texto}</p>
    </div>
  );
}

export function ErrorMsg({ mensaje, onReintentar }) {
  if (!mensaje) return null;
  return (
    <div className="alerta alerta-error">
      <span>{mensaje}</span>
      {onReintentar && (
        <button className="btn btn-small btn-outline" onClick={onReintentar}>Reintentar</button>
      )}
    </div>
  );
}
