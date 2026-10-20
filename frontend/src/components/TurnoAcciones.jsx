import { useState } from "react";
import { turnosApi } from "../api/endpoints.js";
import { mensajeError } from "../api/axios.js";
import { useAuth } from "../context/AuthContext.jsx";

// Botones de acción sobre un turno (Confirmar / Cancelar / Eliminar).
// Se usa tanto en el panel de la agenda como en la tabla de "Mis turnos".
// onChange() le avisa a la página que recargue los datos.
export default function TurnoAcciones({ turno, onChange }) {
  const { isAdmin } = useAuth();
  const [ocupado, setOcupado] = useState(false);
  const [error, setError] = useState("");

  // Ejecuta una acción mostrando "ocupado" y capturando errores del backend
  const ejecutar = async (accion) => {
    setOcupado(true);
    setError("");
    try {
      await accion();
      onChange();
    } catch (err) {
      setError(mensajeError(err));
    } finally {
      setOcupado(false);
    }
  };

  const cancelar = () => {
    if (window.confirm("¿Seguro que querés cancelar este turno?")) {
      ejecutar(() => turnosApi.cambiarEstado(turno.id, "Cancelado"));
    }
  };

  const eliminar = () => {
    if (window.confirm("¿Eliminar definitivamente este turno?")) {
      ejecutar(() => turnosApi.eliminar(turno.id));
    }
  };

  return (
    <div className="acciones">
      {/* Solo el Admin confirma, y solo si está pendiente */}
      {isAdmin && turno.estado === "Pendiente" && (
        <button
          className="btn btn-small btn-success"
          disabled={ocupado}
          onClick={() => ejecutar(() => turnosApi.cambiarEstado(turno.id, "Confirmado"))}
        >
          Confirmar
        </button>
      )}
      {turno.estado !== "Cancelado" && (
        <button className="btn btn-small btn-warning" disabled={ocupado} onClick={cancelar}>
          Cancelar
        </button>
      )}
      <button className="btn btn-small btn-danger" disabled={ocupado} onClick={eliminar}>
        Eliminar
      </button>
      {error && <p className="error-texto">{error}</p>}
    </div>
  );
}
