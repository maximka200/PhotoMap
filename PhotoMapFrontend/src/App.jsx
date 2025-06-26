"use client"

import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom"
import { AuthProvider, useAuth } from "./AuthContext"
import RegistrationForm from "./RegistrationForm"
import LoginForm from "./LoginForm"
import AccountPage from "./pages/AccountPage"
import HomePage from "./pages/HomePage"

// Компонент для защиты маршрутов (только для авторизованных)
const ProtectedRoute = ({ children }) => {
  const { currentUser, loading } = useAuth()

  if (loading) {
    return <div>Загрузка...</div>
  }

  if (!currentUser) {
    return <Navigate to="/login" replace />
  }

  return children
}

// Компонент для гостевых маршрутов (только для неавторизованных)
const GuestRoute = ({ children }) => {
  const { currentUser, loading } = useAuth()

  if (loading) {
    return <div>Загрузка...</div>
  }

  if (currentUser) {
    return <Navigate to="/" replace />
  }

  return children
}

// Главный компонент приложения
export default function App() {
  return (
      <AuthProvider>
        <Router>
          <Routes>
            {/* Главная страница - защищенный маршрут */}
            <Route
                path="/"
                element={
                  <ProtectedRoute>
                    <HomePage />
                  </ProtectedRoute>
                }
            />

            {/* Регистрация - только для гостей */}
            <Route
                path="/register"
                element={
                  <GuestRoute>
                    <RegistrationForm />
                  </GuestRoute>
                }
            />

            {/* Вход - только для гостей */}
            <Route
                path="/login"
                element={
                  <GuestRoute>
                    <LoginForm />
                  </GuestRoute>
                }
            />

            {/* Аккаунт - защищенный маршрут */}
            <Route
                path="/account"
                element={
                  <ProtectedRoute>
                    <AccountPage />
                  </ProtectedRoute>
                }
            />

            {/* Редирект для несуществующих путей */}
            <Route path="*" element={<Navigate to="/login" replace />} />
          </Routes>
        </Router>
      </AuthProvider>
  )
}
