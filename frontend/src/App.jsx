import { Routes, Route, Navigate } from "react-router-dom";
import Layout from "./components/Layout.jsx";
import ProtectedRoute from "./components/ProtectedRoute.jsx";
import LoginPage from "./pages/LoginPage.jsx";
import RegisterPage from "./pages/RegisterPage.jsx";
import AgendaPage from "./pages/AgendaPage.jsx";
import TurnosPage from "./pages/TurnosPage.jsx";
import NuevoTurnoPage from "./pages/NuevoTurnoPage.jsx";
import ServiciosAdminPage from "./pages/ServiciosAdminPage.jsx";

// Define TODAS las rutas de la aplicación (React Router).
export default function App() {
  return (
    <Routes>
      {/* Rutas públicas */}
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      {/* Rutas privadas: ProtectedRoute exige estar logueado; Layout agrega el menú de arriba */}
      <Route
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route path="/" element={<AgendaPage />} />
        <Route path="/turnos" element={<TurnosPage />} />
        <Route path="/turnos/nuevo" element={<NuevoTurnoPage />} />

        {/* Ruta solo para Admin: si entra un Cliente lo manda a "/" */}
        <Route
          path="/admin/servicios"
          element={
            <ProtectedRoute roles={["Admin"]}>
              <ServiciosAdminPage />
            </ProtectedRoute>
          }
        />
      </Route>

      {/* Cualquier otra URL -> inicio */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
