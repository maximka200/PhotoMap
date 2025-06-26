"use client"

import { useState, useEffect } from "react"
import { Link, useNavigate } from "react-router-dom"
import { useAuth } from "../AuthContext"
import PhotoMapAPI from "../apiClient"

const AccountPage = () => {
  const { currentUser, logout, updateProfile } = useAuth()
  const navigate = useNavigate()
  const [showProfileForm, setShowProfileForm] = useState(false)
  const [nickname, setNickname] = useState(currentUser?.userName || "")
  const [avatar, setAvatar] = useState("/user.png")
  const [avatarFile, setAvatarFile] = useState(null)
  const [isLoading, setIsLoading] = useState(false)
  const [avatarLoading, setAvatarLoading] = useState(true)
  const [userStats, setUserStats] = useState({
    photosCount: 0,
    pointsCount: 0,
    likesReceived: 0,
    likesGiven: 0,
  })
  const [statsLoading, setStatsLoading] = useState(true)
  const [userDetails, setUserDetails] = useState(null)

  // Получаем ID пользователя из разных источников
  const getUserId = () => {
    // Сначала пробуем из currentUser
    if (currentUser?.id) return currentUser.id
    if (currentUser?.uId) return currentUser.uId

    // Затем из localStorage
    const storedUser = localStorage.getItem("authUser")
    if (storedUser) {
      try {
        const user = JSON.parse(storedUser)
        return user.id || user.uId
      } catch (e) {
        console.error("Error parsing stored user:", e)
      }
    }

    // Пробуем декодировать токен
    const token = localStorage.getItem("authToken")
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split(".")[1]))
        return payload.sub || payload.userId || payload.id
      } catch (e) {
        console.error("Error decoding token:", e)
      }
    }

    return null
  }

  useEffect(() => {
    if (currentUser) {
      console.log("Current user:", currentUser)
      setNickname(currentUser.userName || "")
      loadUserData()
    }
  }, [currentUser])

  const loadUserData = async () => {
    const userId = getUserId()
    if (!userId) {
      console.error("No user ID found")
      return
    }

    try {
      // Загружаем данные параллельно
      const [avatarResult, statsResult, userResult] = await Promise.allSettled([
        loadUserAvatar(userId),
        loadUserStats(userId),
        loadUserDetails(userId),
      ])

      if (avatarResult.status === "rejected") {
        console.log("Avatar loading failed:", avatarResult.reason)
      }
      if (statsResult.status === "rejected") {
        console.log("Stats loading failed:", statsResult.reason)
      }
      if (userResult.status === "rejected") {
        console.log("User details loading failed:", userResult.reason)
      }
    } catch (error) {
      console.error("Error loading user data:", error)
    }
  }

  const loadUserAvatar = async (userId) => {
    if (!userId) {
      setAvatarLoading(false)
      return
    }

    setAvatarLoading(true)
    try {
      const avatarUrl = await PhotoMapAPI.getAvatar(userId)
      if (avatarUrl) {
        setAvatar(avatarUrl)
      } else {
        setAvatar("/user.png")
      }
    } catch (error) {
      console.log("Аватар не найден, используем стандартный")
      setAvatar("/user.png")
    } finally {
      setAvatarLoading(false)
    }
  }

  const loadUserStats = async (userId) => {
    if (!userId) {
      setStatsLoading(false)
      return
    }

    try {
      const stats = await PhotoMapAPI.getUserStats(userId)
      setUserStats(stats)
    } catch (error) {
      console.error("Ошибка загрузки статистики:", error)
      // Оставляем нулевую статистику
    } finally {
      setStatsLoading(false)
    }
  }

  const loadUserDetails = async (userId) => {
    if (!userId) return

    try {
      const details = await PhotoMapAPI.getUser(userId)
      setUserDetails(details)
    } catch (error) {
      console.error("Ошибка загрузки данных пользователя:", error)
    }
  }

  const handleLogout = () => {
    logout()
    navigate("/login")
  }

  const handleSaveProfile = async () => {
    const userId = getUserId()
    if (!userId) {
      alert("Ошибка: не удалось определить пользователя")
      return
    }

    setIsLoading(true)

    try {
      const updates = {}

      // Обновляем никнейм если изменился
      if (nickname !== currentUser?.userName) {
        updates.userName = nickname
      }

      // Загружаем новый аватар если выбран
      if (avatarFile) {
        await PhotoMapAPI.uploadAvatar(userId, avatarFile)
        // Перезагружаем аватар после загрузки
        await loadUserAvatar(userId)
      }

      // Обновляем профиль если есть изменения
      if (Object.keys(updates).length > 0) {
        await updateProfile(updates)
      }

      setShowProfileForm(false)
      setAvatarFile(null)
      alert("Профиль успешно обновлен!")

      // Перезагружаем данные пользователя
      await loadUserData()
    } catch (error) {
      console.error("Ошибка при сохранении профиля:", error)
      alert("Не удалось сохранить изменения")
    } finally {
      setIsLoading(false)
    }
  }

  const handleAvatarChange = (e) => {
    const file = e.target.files[0]
    if (file) {
      // Проверяем размер файла (максимум 5MB)
      if (file.size > 5 * 1024 * 1024) {
        alert("Размер файла не должен превышать 5MB")
        return
      }

      // Проверяем тип файла
      if (!file.type.startsWith("image/")) {
        alert("Пожалуйста, выберите изображение")
        return
      }

      const reader = new FileReader()
      reader.onloadend = () => {
        setAvatar(reader.result)
        setAvatarFile(file)
      }
      reader.readAsDataURL(file)
    }
  }

  const handleDeleteAvatar = async () => {
    const userId = getUserId()
    if (!userId) return

    if (!confirm("Удалить аватар?")) return

    setIsLoading(true)

    try {
      await PhotoMapAPI.deleteAvatar()
      setAvatar("/user.png")
      setAvatarFile(null)
      alert("Аватар удален")
    } catch (error) {
      console.error("Ошибка при удалении аватара:", error)
      alert("Не удалось удалить аватар")
    } finally {
      setIsLoading(false)
    }
  }

  const handleCancelEdit = () => {
    setShowProfileForm(false)
    setNickname(currentUser?.userName || "")
    setAvatarFile(null)
    // Восстанавливаем оригинальный аватар
    const userId = getUserId()
    if (userId) {
      loadUserAvatar(userId)
    }
  }

  const formatDate = (dateString) => {
    if (!dateString) return "Не указано"
    try {
      return new Date(dateString).toLocaleDateString("ru-RU", {
        year: "numeric",
        month: "long",
        day: "numeric",
      })
    } catch {
      return "Не указано"
    }
  }

  return (
      <div className="account-page">
        <header className="account-header">
          <div className="header-content">
            <div className="logo-section">
              <img className="header-logo" src="/pen.png" alt="PhotoFinder" />
              <span className="app-title">PhotoFinder</span>
            </div>

            <div className="header-actions">
              <Link className="back-link" to="/">
                ← Карта
              </Link>
              <button className="logout-btn" onClick={handleLogout}>
                Выход
              </button>
            </div>
          </div>
        </header>

        <main className="account-main">
          <div className="account-container">
            <div className="profile-section">
              <div className="profile-header">
                <div className="avatar-container">
                  {avatarLoading ? (
                      <div className="profile-avatar loading">
                        Загрузка...
                      </div>
                  ) : (
                      <img
                          className="profile-avatar"
                          src={avatar}
                          alt="Аватар пользователя"
                          onError={(e) => {
                            e.currentTarget.onerror = null
                            e.currentTarget.src = "/user.png"
                          }}
                      />
                  )}

                  {showProfileForm && (
                      <>
                        <input
                            type="file"
                            accept="image/*"
                            onChange={handleAvatarChange}
                            id="avatar-upload"
                            className="avatar-input"
                        />
                        <label htmlFor="avatar-upload" className="avatar-edit-btn" title="Изменить аватар">
                          📷
                        </label>

                        {avatar && avatar !== "/user.png" && !avatar.startsWith("data:") && (
                            <button
                                className="avatar-delete-btn"
                                onClick={handleDeleteAvatar}
                                title="Удалить аватар"
                                disabled={isLoading}
                            >
                              🗑️
                            </button>
                        )}
                      </>
                  )}
                </div>

                <div className="profile-info">
                  {!showProfileForm ? (
                      <>
                        <h1 className="profile-name">{currentUser?.userName || "Пользователь"}</h1>
                        <p className="profile-email">{currentUser?.email || userDetails?.email || "email@example.com"}</p>
                        {userDetails?.createdAt && (
                            <p
                                className="profile-join-date"
                                style={{ fontSize: '12px', lineHeight: '1.8' }}
                            >
                              Регистрация: {formatDate(userDetails.createdAt)}
                            </p>
                        )}
                      </>
                  ) : (
                      <div className="edit-form">
                        <div className="form-field">
                          <label htmlFor="nickname">Имя пользователя</label>
                          <input
                              type="text"
                              id="nickname"
                              value={nickname}
                              onChange={(e) => setNickname(e.target.value)}
                              className="name-input"
                              placeholder="Введите имя"
                              disabled={isLoading}
                          />
                        </div>
                        {avatarFile && (
                            <div className="avatar-preview-info">
                              <span className="preview-text">Новый аватар выбран</span>
                              <small>Файл: {avatarFile.name}</small>
                            </div>
                        )}
                      </div>
                  )}
                </div>
              </div>

              {!showProfileForm ? (
                  <button className="edit-profile-btn" onClick={() => setShowProfileForm(true)}>
                    Редактировать
                  </button>
              ) : (
                  <div className="form-actions">
                    <button onClick={handleSaveProfile} className="save-btn" disabled={isLoading}>
                      {isLoading ? "Сохранение..." : "Сохранить"}
                    </button>
                    <button onClick={handleCancelEdit} className="cancel-btn" disabled={isLoading}>
                      Отмена
                    </button>
                  </div>
              )}
            </div>
          </div>
        </main>
      </div>
  )
}

export default AccountPage