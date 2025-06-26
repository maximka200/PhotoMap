import axios from "axios"

const API_BASE_URL = "http://localhost:5001"

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
})

apiClient.interceptors.request.use(
    (config) => {
      const token = localStorage.getItem("authToken")
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      }
      return config
    },
    (error) => {
      return Promise.reject(error)
    },
)

// Добавляем interceptor для обработки ошибок авторизации
apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
      if (error.response?.status === 401) {
        // Токен недействителен, очищаем localStorage
        localStorage.removeItem("authToken")
        localStorage.removeItem("currentUser")

        // Перенаправляем на страницу входа, если не находимся на ней
        if (window.location.pathname !== "/login" && window.location.pathname !== "/register") {
          window.location.href = "/login"
        }
      }
      return Promise.reject(error)
    },
)

const handleApiError = (error) => {
  if (error.response) {
    throw error.response.data
  } else if (error.request) {
    throw new Error("No response received from server")
  } else {
    throw new Error("Error setting up the request: " + error.message)
  }
}

// Функция для форматирования фотографий
const formatPhoto = (photo) => {
  if (!photo) return null
  
  return {
    id: photo.uId,
    url: photo.url.startsWith("http") ? photo.url : `${API_BASE_URL}${photo.url}`,
    pointId: photo.pointId,
    createdAt: photo.createdAt,
    likes: photo.likedIds ? photo.likedIds.length : 0,
    likedIds: photo.likedIds || [],
    isLiked: false, // Будет обновлено при проверке текущего пользователя
  }
}

// Функция для форматирования отзывов
const formatReview = (review) => {
  if (!review) return null

  return {
    id: review.uId || review.id,
    text: review.text || review.comment,
    username: review.username || review.author || review.userName,
    date: review.createdAt || review.date,
    likes: review.likedByUsers ? review.likedByUsers.length : review.likes || 0,
    photos: review.photos ? review.photos.map(formatPhoto).filter(Boolean) : [],
  }
}

const PhotoMapAPI = {
  // Аутентификация
  login: async (credentials) => {
    try {
      const response = await apiClient.post("/api/auth/login", credentials)
      return response.data
    } catch (error) {
      handleApiError(error)
    }
  },

  register: async (userData) => {
    try {
      const response = await apiClient.post("/api/auth/register", userData)
      return response.data
    } catch (error) {
      handleApiError(error)
    }
  },

  // Работа с аватарами
  uploadAvatar: async (userId, file) => {
    try {
      const formData = new FormData()
      formData.append("file", file)

      const response = await apiClient.post(`/api/avatar/upload?userId=${userId}`, formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      return response.data
    } catch (error) {
      handleApiError(error)
    }
  },

  deleteAvatar: async () => {
    try {
      const response = await apiClient.delete("/api/avatar/delete")
      return response.data
    } catch (error) {
      handleApiError(error)
    }
  },

  getAvatar: async (userId) => {
    try {
      const response = await apiClient.get(`avatars/${userId}.jpg`, {
        responseType: "blob",
      })
      return URL.createObjectURL(response.data)
    } catch (error) {
      handleApiError(error)
    }
  },
  

  deletePhoto: async (id) => {
    try {
      const response = await apiClient.delete(`/api/photos/${id}`)
      return response.data
    } catch (error) {
      handleApiError(error)
    }
  },

  // Загрузка фотографии на точку
  uploadPhoto: async (pointId, file) => {
    try {
      const formData = new FormData()
      formData.append("file", file)

      const response = await apiClient.post(`/api/photos/upload?pointId=${pointId}`, formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })

      // Форматируем ответ согласно новому формату
      return formatPhoto(response.data)
    } catch (error) {
      handleApiError(error)
    }
  },

  // Загрузка нескольких фотографий
  uploadMultiplePhotos: async (pointId, files) => {
    try {
      const uploadPromises = files.map((file) => PhotoMapAPI.uploadPhoto(pointId, file))
      const results = await Promise.all(uploadPromises)
      return results.filter(Boolean) // Убираем null значения
    } catch (error) {
      handleApiError(error)
    }
  },

  // Методы для работы с лайками фотографий
  likePhoto: async (photoId) => {
    try {
      const response = await apiClient.post(`/api/photos/like/${photoId}`)
      return response.data
    } catch (error) {
      handleApiError(error)
    }
  },

  dislikePhoto: async (photoId) => {
    try {
      const response = await apiClient.delete(`/api/photos/dislike/${photoId}`)
      return response.data
    } catch (error) {
      handleApiError(error)
    }
  },

  // Получение понравившихся фотографий пользователя
  getLikedPhotos: async () => {
    try {
      const response = await apiClient.get("/api/user/liked")
      return response.data.map(formatPhoto).filter(Boolean)
    } catch (error) {
      handleApiError(error)
    }
  },

  // Проверка лайков пользователя для массива фотографий
  checkUserLikes: async (photos, currentUserId) => {
    if (!currentUserId || !photos || photos.length === 0) {
      return photos
    }

    return photos.map((photo) => ({
      ...photo,
      isLiked: photo.likedIds && photo.likedIds.includes(currentUserId),
    }))
  },

  // Работа с точками
  getPointsInEkaterinburg: async () => {
    try {
      const response = await apiClient.get("/api/points/ekaterinburg")

      const points = await Promise.all(
          response.data.map(async (point) => {
            const photos = point.photos
                ? await Promise.all(point.photos.map((photo) => PhotoMapAPI.getPhotoById(photo.uId)))
                : []

            const reviews = point.reviews
                ? point.reviews.map(formatReview).filter(Boolean)
                : []

            return {
              id: point.uId,
              name: point.name,
              latitude: point.latitude,
              longitude: point.longitude,
              photos,
              reviews,
            }
          })
      )

      return points
    } catch (error) {
      handleApiError(error)
      return [] // на случай ошибки — возвращаем пустой массив
    }
  },

  getPointDetails: async (id) => {
    try {
      const response = await apiClient.get(`/api/points/${id}`)

      const formattedPhotos = response.data.photos ? await Promise.all(response.data.photos.map((photo) => PhotoMapAPI.getPhotoById(photo.uId))) : []
      return {
        id: response.data.uId,
        position: [response.data.longitude, response.data.latitude],
        address: response.data.name,
        photos: formattedPhotos,
        reviews: response.data.reviews ? response.data.reviews.map(formatReview).filter(Boolean) : [],
      }
    } catch (error) {
      handleApiError(error)
    }
  },

  deletePoint: async (id) => {
    try {
      const response = await apiClient.delete(`/api/points/${id}`)
      return response.data
    } catch (error) {
      handleApiError(error)
    }
  },

  createPoint: async (pointData) => {
    try {
      const response = await apiClient.post("/api/points", pointData)
      return {
        ...response.data,
        id: response.data.uId,
      }
    } catch (error) {
      handleApiError(error)
    }
  },

  // Работа с пользователями
  getUser: async (id) => {
    try {
      const response = await apiClient.get(`/api/user/${id}`)
      return response.data
    } catch (error) {
      handleApiError(error)
    }
  },

  getAllUsers: async () => {
    try {
      const response = await apiClient.get("/api/user/all")
      return response.data
    } catch (error) {
      handleApiError(error)
    }
  },

  // Получение текущего пользователя - используем /api/user/all и фильтруем по токену
  getCurrentUser: async () => {
    try {
      // Поскольку нет эндпоинта для текущего пользователя,
      // используем информацию из токена или другой способ
      const token = localStorage.getItem("authToken")
      if (!token) {
        throw new Error("No auth token")
      }

      // Декодируем токен чтобы получить ID пользователя
      try {
        const payload = JSON.parse(atob(token.split(".")[1]))
        const userId = payload.sub || payload.userId || payload.id

        if (userId) {
          return await PhotoMapAPI.getUser(userId)
        }
      } catch (tokenError) {
        console.error("Failed to decode token:", tokenError)
      }

      // Fallback: получаем всех пользователей и ищем текущего
      // Это не оптимально, но работает при отсутствии нужного эндпоинта
      const users = await PhotoMapAPI.getAllUsers()

      // Возвращаем первого пользователя как fallback
      // В реальном приложении нужен эндпоинт /api/user/me
      if (users && users.length > 0) {
        return users[0]
      }

      throw new Error("No current user found")
    } catch (error) {
      handleApiError(error)
    }
  },

  // Вычисление статистики пользователя вручную (нет API эндпоинта)
  calculateUserStats: async (userId) => {
    try {
      // Получаем все точки и фотографии для подсчета статистики
      const [points, likedPhotos] = await Promise.all([
        PhotoMapAPI.getPointsInEkaterinburg().catch(() => []),
        PhotoMapAPI.getLikedPhotos().catch(() => []),
      ])

      // Фильтруем точки и фотографии пользователя
      const userPoints = points.filter((point) => point.createdBy === userId) || []
      const userPhotos = points.reduce((acc, point) => {
        const pointPhotos = point.photos?.filter((photo) => photo.createdBy === userId) || []
        return [...acc, ...pointPhotos]
      }, [])

      const totalLikes = userPhotos.reduce((sum, photo) => sum + (photo.likes || 0), 0)

      return {
        photosCount: userPhotos.length,
        pointsCount: userPoints.length,
        likesReceived: totalLikes,
        likesGiven: likedPhotos.length,
      }
    } catch (error) {
      console.error("Error calculating user stats:", error)
      return {
        photosCount: 0,
        pointsCount: 0,
        likesReceived: 0,
        likesGiven: 0,
      }
    }
  },

  // Получение статистики пользователя
  getUserStats: async (userId) => {
    // Поскольку нет API эндпоинта для статистики, вычисляем вручную
    return await PhotoMapAPI.calculateUserStats(userId)
  },

  // Получение фотографий пользователя (вычисляем из всех точек)
  getUserPhotos: async (userId) => {
    try {
      const points = await PhotoMapAPI.getPointsInEkaterinburg()
      const userPhotos = points.reduce((acc, point) => {
        const pointPhotos = point.photos?.filter((photo) => photo.createdBy === userId) || []
        return [...acc, ...pointPhotos]
      }, [])

      return userPhotos
    } catch (error) {
      console.error("Error getting user photos:", error)
      return []
    }
  },

  // Получение точек пользователя (вычисляем из всех точек)
  getUserPoints: async (userId) => {
    try {
      const points = await PhotoMapAPI.getPointsInEkaterinburg()
      const userPoints = points
          .filter((point) => point.createdBy === userId)
          .map((point) => ({
            id: point.id,
            name: point.name,
            latitude: point.latitude,
            longitude: point.longitude,
            createdAt: point.createdAt,
            photosCount: point.photos ? point.photos.length : 0,
          }))

      return userPoints
    } catch (error) {
      console.error("Error getting user points:", error)
      return []
    }
  },

  // Обновление профиля пользователя (только аватар доступен)
  updateProfile: async (userId, profileData) => {
    try {
      const updateOperations = []

      if (profileData.avatar) {
        updateOperations.push(PhotoMapAPI.uploadAvatar(userId, profileData.avatar))
      }

      // Другие поля профиля нельзя обновить через API
      if (profileData.displayName || profileData.userName) {
        console.warn("Username/displayName update not supported by API")
      }

      const results = await Promise.all(updateOperations)
      return results
    } catch (error) {
      handleApiError(error)
    }
  },

  getPhotoById: async (id) => {
    try {
      const response = await apiClient.get(`/api/photos/${id}`);
      return formatPhoto(response.data);
    } catch (error) {
      handleApiError(error);
    }
  },
  
}

export default PhotoMapAPI
