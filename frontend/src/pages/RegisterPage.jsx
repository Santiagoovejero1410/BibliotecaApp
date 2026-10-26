import { useState } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext.jsx";
import { getError } from "../api/api.js";

export default function RegisterPage() {
  const { user, register } = useAuth();
  const navigate = useNavigate();

  const [form, setForm] = useState({ nombre: "", email: "", password: "", confirmar: "" });
  const [errores, setErrores] = useState({});
  const [errorApi, setErrorApi] = useState("");
  const [cargando, setCargando] = useState(false);

  if (user) return <Navigate to="/" replace />;

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  const validar = () => {
    const e = {};
    if (form.nombre.trim().length < 2) e.nombre = "El nombre debe tener al menos 2 caracteres";
    if (!form.email.trim()) e.email = "El email es obligatorio";
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) e.email = "El email no es válido";
    if (form.password.length < 6) e.password = "La contraseña debe tener al menos 6 caracteres";
    if (form.confirmar !== form.password) e.confirmar = "Las contraseñas no coinciden";
    return e;
  };

  const handleSubmit = async (ev) => {
    ev.preventDefault();
    const e = validar();
    setErrores(e);
    if (Object.keys(e).length > 0) return;

    setCargando(true);
    setErrorApi("");
    try {
      // El backend crea el usuario con rol Socio y devuelve el token: queda logueado
      await register(form.nombre, form.email, form.password);
      navigate("/");
    } catch (err) {
      setErrorApi(getError(err));
    } finally {
      setCargando(false);
    }
  };

  return (
    <div className="auth-page">
      <form className="card auth-card" onSubmit={handleSubmit} noValidate>
        <h1>Crear cuenta</h1>

        {errorApi && <p className="alerta">{errorApi}</p>}

        <label>Nombre</label>
        <input name="nombre" value={form.nombre} onChange={handleChange} />
        {errores.nombre && <p className="error-texto">{errores.nombre}</p>}

        <label>Email</label>
        <input name="email" type="email" value={form.email} onChange={handleChange} />
        {errores.email && <p className="error-texto">{errores.email}</p>}

        <label>Contraseña</label>
        <input name="password" type="password" value={form.password} onChange={handleChange} />
        {errores.password && <p className="error-texto">{errores.password}</p>}

        <label>Repetir contraseña</label>
        <input name="confirmar" type="password" value={form.confirmar} onChange={handleChange} />
        {errores.confirmar && <p className="error-texto">{errores.confirmar}</p>}

        <button className="btn btn-primary btn-block" disabled={cargando}>
          {cargando ? "Creando..." : "Registrarme"}
        </button>
        <p className="muted">¿Ya tenés cuenta? <Link to="/login">Iniciá sesión</Link></p>
      </form>
    </div>
  );
}
