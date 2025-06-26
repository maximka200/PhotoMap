"use client"

import { useState } from "react"
import { useNavigate, Link } from "react-router-dom"
import { useAuth } from "./AuthContext"
import axios from "axios"

const RegistrationForm = () => {
  const navigate = useNavigate()
  const [formData, setFormData] = useState({
    user: "",
    email: "",
    password: "",
  })
  const [errors, setErrors] = useState({})
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [registrationSuccess, setRegistrationSuccess] = useState(false)
  const { login } = useAuth()

  const validateForm = () => {
    const newErrors = {}
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    const existingUsers = JSON.parse(localStorage.getItem("users") || "[]")

    if (!formData.user.trim()) {
      newErrors.user = "Имя пользователя обязательно"
    } else if (existingUsers.some((user) => user.user === formData.user)) {
      newErrors.user = "Имя пользователя уже занято"
    }

    if (!emailRegex.test(formData.email)) {
      newErrors.email = "Некорректный email"
    }

    if (formData.password.length < 6) {
      newErrors.password = "Пароль должен быть не менее 6 символов"
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!validateForm()) return

    setIsSubmitting(true)

    try {
      // Отправка данных на сервер
      const response = await axios.post("http://localhost:5001/api/auth/register", {
        userName: formData.user,
        email: formData.email,
        password: formData.password,
      })

      if (response.status === 200) {
        setRegistrationSuccess(true)
        localStorage.setItem(
            "currentUser",
            JSON.stringify({
              userName: formData.user,
              email: formData.email,
            }),
        )
        setTimeout(() => {
          navigate("/login")
        }, 1500)
      }
    } catch (error) {
      console.error("Ошибка регистрации:", error)
      setErrors({ server: "Ошибка регистрации. Попробуйте позже." })
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
            <p className="auth-subtitle">Создайте новый аккаунт</p>
          </div>

          <form onSubmit={handleSubmit} className="auth-form">
            <div className="form-field">
              <label htmlFor="user">Имя пользователя</label>
              <input
                  type="text"
                  id="user"
                  name="user"
                  value={formData.user}
                  onChange={handleChange}
                  className={errors.user ? "error" : ""}
                  placeholder="Введите имя пользователя"
              />
              {errors.user && <span className="field-error">{errors.user}</span>}
            </div>

            <div className="form-field">
              <label htmlFor="email">Email</label>
              <input
                  type="email"
                  id="email"
                  name="email"
                  value={formData.email}
                  onChange={handleChange}
                  className={errors.email ? "error" : ""}
                  placeholder="Введите email"
              />
              {errors.email && <span className="field-error">{errors.email}</span>}
            </div>

            <div className="form-field">
              <label htmlFor="password">Пароль</label>
              <input
                  type="password"
                  id="password"
                  name="password"
                  value={formData.password}
                  onChange={handleChange}
                  className={errors.password ? "error" : ""}
                  placeholder="Введите пароль (минимум 6 символов)"
              />
              {errors.password && <span className="field-error">{errors.password}</span>}
            </div>

            {errors.server && <div className="error-message">{errors.server}</div>}

            <button type="submit" disabled={isSubmitting} className="auth-button">
              {isSubmitting ? "Регистрация..." : "Зарегистрироваться"}
            </button>

            {registrationSuccess && (
                <div className="success-message">Регистрация успешна! Перенаправляем на страницу входа...</div>
            )}
          </form>

          <div className="auth-footer">
            <p>
              Уже есть аккаунт? <Link to="/login">Войти</Link>
            </p>
          </div>
        </div>
      </div>
  )
}

export default RegistrationForm
