import { Routes, Route, Navigate } from "react-router-dom";
import Navbar from "./components/Navbar.jsx";
import ProtectedRoute from "./components/ProtectedRoute.jsx";
import LoginPage from "./pages/LoginPage.jsx";
import RegisterPage from "./pages/RegisterPage.jsx";
import LibrosPage from "./pages/LibrosPage.jsx";
import LibroFormPage from "./pages/LibroFormPage.jsx";
import PrestamosPage from "./pages/PrestamosPage.jsx";

// Define todas las rutas de la aplicación (React Router).
export default function App() {
  return (
    <>
      <Navbar />
      <Routes>
        {/* Rutas públicas */}
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        {/* Rutas privadas: hay que estar logueado */}
        <Route path="/" element={<ProtectedRoute><LibrosPage /></ProtectedRoute>} />
        <Route path="/prestamos" element={<ProtectedRoute><PrestamosPage /></ProtectedRoute>} />

        {/* Rutas solo para Admin: si entra un Socio, lo manda al inicio */}
        <Route path="/libros/nuevo" element={<ProtectedRoute roles={["Admin"]}><LibroFormPage /></ProtectedRoute>} />
        <Route path="/libros/:id/editar" element={<ProtectedRoute roles={["Admin"]}><LibroFormPage /></ProtectedRoute>} />

        {/* Cualquier otra URL -> inicio */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </>
  );
}
