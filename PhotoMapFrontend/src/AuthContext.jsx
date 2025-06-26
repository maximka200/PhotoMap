"use client"

import { createContext, useContext, useEffect, useState } from "react"
import PhotoMapAPI from "./apiClient"

const AuthContext = createContext()

export const AuthProvider = ({ children }) => {
  const [currentUser, setCurrentUser] = useState(null)
  const [loading, setLoading] = useState(true)
  const [isAuthenticated, setIsAuthenticated] = useState(false)

  useEffect(() => {
    initializeAuth()
  }, [])

  const initializeAuth = async () => {
    try {
      const token = localStorage.getItem("authToken")
      const savedUser = localStorage.getItem("currentUser")

      if (!token) {
        setLoading(false)
        return
      }

      // Если есть токен, пытаемся получить данные пользователя
      try {
        const userData = await PhotoMapAPI.getCurrentUser()

        if (userData) {
          const formattedUser = {
            id: userData.id || userData.uId,
            userName: userData.userName || userData.username,
            email: userData.email,
            createdAt: userData.createdAt,
            ...userData,
          }

          setCurrentUser(formattedUser)
          setIsAuthenticated(true)

          // Обновляем localStorage с актуальными данными
          localStorage.setItem("currentUser", JSON.stringify(formattedUser))
        } else {
          throw new Error("No user data received")
        }
      } catch (apiError) {
        console.error("Failed to fetch user from API:", apiError)

        // Если API недоступен, пытаемся использовать сохраненные данные
        if (savedUser) {
          try {
            const parsedUser = JSON.parse(savedUser)
            setCurrentUser(parsedUser)
            setIsAuthenticated(true)
            console.log("Using cached user data")
          } catch (parseError) {
            console.error("Failed to parse saved user:", parseError)
            logout()
          }
        } else {
          // Если нет сохраненных данных и API недоступен, выходим
          logout()
        }
      }
    } catch (error) {
      console.error("Auth initialization error:", error)
      logout()
    } finally {
      setLoading(false)
    }
  }

  const login = async (userData) => {
    try {
      setLoading(true)

      // Сохраняем токен
      if (userData.token) {
        localStorage.setItem("authToken", userData.token)
      }

      // Получаем полные данные пользователя
      let userInfo = userData.user || userData

      // Если данных пользователя нет в ответе логина, запрашиваем их
      if (!userInfo || !userInfo.id) {
        try {
          userInfo = await PhotoMapAPI.getCurrentUser()
        } catch (error) {
          console.error("Failed to fetch user after login:", error)
          // Используем данные из ответа логина или создаем базовые
          userInfo = userData.user || {
            userName: userData.userName,
            email: userData.email,
            id: userData.id,
          }
        }
      }

      const formattedUser = {
        id: userInfo.id || userInfo.uId,
        userName: userInfo.userName || userInfo.username,
        email: userInfo.email,
        createdAt: userInfo.createdAt,
        ...userInfo,
      }

      setCurrentUser(formattedUser)
      setIsAuthenticated(true)

      // Сохраняем в localStorage
      localStorage.setItem("currentUser", JSON.stringify(formattedUser))

      return formattedUser
    } catch (error) {
      console.error("Login error:", error)
      throw error
    } finally {
      setLoading(false)
    }
  }

  const logout = () => {
    localStorage.removeItem("authToken")
    localStorage.removeItem("currentUser")
    setCurrentUser(null)
    setIsAuthenticated(false)
  }

  // Обновление профиля (ограниченное API)
  const updateProfile = async (updates) => {
    if (!currentUser) {
      throw new Error("No current user")
    }

    try {
      setLoading(true)

      // Обновляем только аватар через API
      if (updates.avatar && currentUser.id) {
        await PhotoMapAPI.updateProfile(currentUser.id, { avatar: updates.avatar })
      }

      // Остальные поля обновляем локально (так как API не поддерживает)
      const updatedUser = {
        ...currentUser,
        ...updates,
        updatedAt: new Date().toISOString(),
      }

      // Убираем avatar из локального обновления
      delete updatedUser.avatar

      setCurrentUser(updatedUser)
      localStorage.setItem("currentUser", JSON.stringify(updatedUser))

      return updatedUser
    } catch (error) {
      console.error("Profile update error:", error)
      throw error
    } finally {
      setLoading(false)
    }
  }

  // Проверка валидности токена
  const checkTokenValidity = async () => {
    const token = localStorage.getItem("authToken")
    if (!token) {
      return false
    }

    try {
      await PhotoMapAPI.getCurrentUser()
      return true
    } catch (error) {
      console.error("Token validation failed:", error)
      logout()
      return false
    }
  }

  // Обновление данных пользователя
  const refreshUser = async () => {
    if (!isAuthenticated) {
      return null
    }

    try {
      const userData = await PhotoMapAPI.getCurrentUser()

      if (userData) {
        const formattedUser = {
          id: userData.id || userData.uId,
          userName: userData.userName || userData.username,
          email: userData.email,
          createdAt: userData.createdAt,
          ...userData,
        }

        setCurrentUser(formattedUser)
        localStorage.setItem("currentUser", JSON.stringify(formattedUser))
        return formattedUser
      }
    } catch (error) {
      console.error("Failed to refresh user:", error)
      // Не выходим из системы при ошибке обновления
    }

    return currentUser
  }

  const value = {
    currentUser,
    isAuthenticated,
    loading,
    login,
    logout,
    updateProfile,
    checkTokenValidity,
    refreshUser,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export const useAuth = () => {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider")
  }
  return context
}
