import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext.jsx";

// Protege rutas. Sin usuario -> /login. Si la ruta pide un rol y el usuario no lo tiene -> inicio.
// Uso: <ProtectedRoute roles={["Admin"]}> ... </ProtectedRoute>
export default function ProtectedRoute({ children, roles }) {
  const { user } = useAuth();

  if (!user) return <Navigate to="/login" replace />;
  if (roles && !roles.includes(user.rol)) return <Navigate to="/" replace />;

  return children;
}
