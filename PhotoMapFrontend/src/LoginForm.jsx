"use client"

import { useState } from "react"
import { useNavigate, Link } from "react-router-dom"
import { useAuth } from "./AuthContext"
import PhotoMapAPI from "./apiClient"

const LoginForm = () => {
  const navigate = useNavigate()
  const { login } = useAuth()
  const [formData, setFormData] = useState({
    userName: "",
    password: "",
  })
  const [error, setError] = useState("")
  const [isSubmitting, setIsSubmitting] = useState(false)

  const handleSubmit = async (e) => {
    e.preventDefault()
    setIsSubmitting(true)
    setError("")

    try {
      // Вызываем API для аутентификации
      const response = await PhotoMapAPI.login({
        userName: formData.userName,
        password: formData.password,
      })

      // Проверяем наличие токена в ответе
      if (response && response.token) {
        // Используем метод login из AuthContext
        await login(response)
        navigate("/")
      } else {
        throw new Error("Неверный ответ сервера")
      }
    } catch (err) {
      console.error("Login error:", err)
      setError(err.message || err.response?.data?.message || "Ошибка при входе")
    } finally {
      setIsSubmitting(false)
    }
  }

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    })
  }

  return (
      <div className="auth-page">
        <div className="auth-container">
          <div className="auth-header">
            <div className="logo-section">
              <img src="/pen.png" alt="PhotoFinder" className="auth-logo" />
              <h1>PhotoFinder</h1>
            </div>
            <p className="auth-subtitle">Войдите в свой аккаунт</p>
          </div>

          <form onSubmit={handleSubmit} className="auth-form">
            <div className="form-field">
              <label htmlFor="userName">Имя пользователя</label>
              <input
                  type="text"
                  id="userName"
                  name="userName"
                  value={formData.userName}
                  onChange={handleChange}
                  required
                  placeholder="Введите имя пользователя"
                  disabled={isSubmitting}
              />
            </div>

            <div className="form-field">
              <label htmlFor="password">Пароль</label>
              <input
                  type="password"
                  id="password"
                  name="password"
                  value={formData.password}
                  onChange={handleChange}
                  required
                  placeholder="Введите пароль"
                  disabled={isSubmitting}
                  disabled={isSubmitting}
              />
            </div>

            {error && <div className="error-message">{error}</div>}

            <button type="submit" disabled={isSubmitting} className="auth-button">
              {isSubmitting ? "Вход..." : "Войти"}
            </button>
          </form>

          <div className="auth-footer">
            <p>
              Нет аккаунта? <Link to="/register">Зарегистрироваться</Link>
            </p>
          </div>
        </div>
      </div>
  )
}

export default LoginForm
