"use client"

import { useState, useEffect, useRef } from "react"
import { load } from "@2gis/mapgl"
import axios from "axios"
import Modal from "react-modal"
import { useAuth } from "./AuthContext"
import PhotoMapAPI from "./apiClient"
import "../styles/index.css"
import "../styles/PlaceInfoModal.css"

Modal.setAppElement("#root")

const DGIS_API_KEY = "edda6e79-252f-4f06-ad10-3e5babae9927"
const GEOCODE_URL = "https://catalog.api.2gis.com/3.0/items/geocode"
const DIRECTIONS_URL = "https://routing.api.2gis.com/carrouting/6.0.0/global"

const MapWithControls = () => {
  const mapContainerRef = useRef(null)
  const mapInstance = useRef(null)
  const mapglAPI = useRef(null)
  const [markers, setMarkers] = useState([])
  const [fullscreenPhoto, setFullscreenPhoto] = useState(null)
  const [likedPhotoIds, setLikedPhotoIds] = useState({})
  const [isAddingMode, setIsAddingMode] = useState(false)
  const [tempMarker, setTempMarker] = useState(null)
  const [selectedLocation, setSelectedLocation] = useState(null)
  const [newPhotos, setNewPhotos] = useState([])
  const [routeStartAddress, setRouteStartAddress] = useState("")
  const [showRouteModal, setShowRouteModal] = useState(false)
  const [routeError, setRouteError] = useState("")
  const [routeData, setRouteData] = useState(null)
  const [mapLoaded, setMapLoaded] = useState(false)
  const [isUploading, setIsUploading] = useState(false)
  const isAddingModeRef = useRef(isAddingMode)
  const { currentUser } = useAuth()
  const [routeDrawn, setRouteDrawn] = useState(false)

  // Загрузка карты
  useEffect(() => {
    if (!mapContainerRef.current) return

    let map
    let isMounted = true

    const initMap = async () => {
      try {
        const api = await load()
        if (!isMounted) return

        mapglAPI.current = api

        map = new api.Map(mapContainerRef.current, {
          center: [60.597465, 56.838011],
          zoom: 13,
          key: DGIS_API_KEY,
        })

        mapInstance.current = map
        setMapLoaded(true)

        map.on("click", handleMapClick)
      } catch (error) {
        console.error("Ошибка инициализации карты:", error)
        alert("Не удалось загрузить карту. Проверьте подключение к интернету.")
      }
    }

    initMap()

    return () => {
      isMounted = false
      if (map) {
        map.off("click", handleMapClick)
        map.destroy()
        mapInstance.current = null
      }
    }
  }, [])

  useEffect(() => {
    isAddingModeRef.current = isAddingMode
  }, [isAddingMode])

  const handleCancelAdding = () => {
    setIsAddingMode(false)

    if (mapInstance.current?.tempMarker) {
      mapInstance.current.tempMarker.destroy()
      mapInstance.current.tempMarker = null
    }

    setTempMarker(null)
  }

  const toggleAddingMode = () => {
    if (isAddingMode) {
      handleCancelAdding()
    } else {
      setIsAddingMode(true)
    }
  }

  // Загрузка точек при монтировании
  useEffect(() => {
    const fetchPoints = async () => {
      try {
        const points = await PhotoMapAPI.getPointsInEkaterinburg()
        console.log("Загруженные точки с фотографиями:", points)

        const formattedPoints = points.map((point) => ({
          id: point.id,
          position: [point.longitude, point.latitude],
          address: point.name,
          photos: point.photos || [],
        }))

        setMarkers(formattedPoints)
      } catch (error) {
        console.error("Ошибка загрузки точек:", error)
      }
    }

    if (mapLoaded) {
      fetchPoints()
    }
  }, [mapLoaded])

  // Обновление маркеров на карте
  useEffect(() => {
    if (!mapInstance.current || !mapglAPI.current || !mapLoaded) {
      return
    }

    // Удаление всех текущих маркеров
    if (mapInstance.current.markers) {
      mapInstance.current.markers.forEach((marker) => marker.destroy())
    }
    mapInstance.current.markers = []

    if (mapInstance.current.tempMarker) {
      mapInstance.current.tempMarker.destroy()
      mapInstance.current.tempMarker = null
    }

    // Добавление постоянных маркеров
    markers.forEach((marker) => {
      try {
        const dgMarker = new mapglAPI.current.Marker(mapInstance.current, {
          coordinates: marker.position,
        })

        dgMarker.on("click", (e) => {
          handleMarkerClick(marker)
        })

        mapInstance.current.markers.push(dgMarker)
      } catch (error) {
        console.error("Ошибка создания маркера:", error)
      }
    })

    // Добавление временного маркера
    if (tempMarker) {
      try {
        const tempDgMarker = new mapglAPI.current.Marker(mapInstance.current, {
          coordinates: tempMarker.position,
        })
        mapInstance.current.tempMarker = tempDgMarker
      } catch (error) {
        console.error("Ошибка создания временного маркера:", error)
      }
    }
  }, [markers, tempMarker, mapLoaded])

  const handleMapClick = async (e) => {
    setTimeout(async () => {
      if (!isAddingModeRef.current || !mapInstance.current) return

      const [lng, lat] = e.lngLat

      const token = localStorage.getItem("authToken")
      if (!token) {
        alert("Для добавления точек требуется авторизация")
        setIsAddingMode(false)
        return
      }

      setTempMarker({
        position: [lng, lat],
        address: "Определение адреса...",
      })

      let address = "Адрес не определен"
      try {
        const { data } = await axios.get(GEOCODE_URL, {
          params: {
            lat,
            lon: lng,
            fields: "items.point,items.full_name",
            key: DGIS_API_KEY,
          },
        })

        if (data?.result?.items?.length > 0) {
          address = data.result.items[0].full_name || "Адрес не определен"
        }
      } catch (error) {
        console.error("Ошибка геокодирования:", error)
      }

      setTempMarker({
        position: [lng, lat],
        address,
      })

      try {
        const newPoint = await PhotoMapAPI.createPoint({
          name: address,
          latitude: lat,
          longitude: lng,
        })

        setMarkers((prev) => [
          ...prev,
          {
            id: newPoint.id,
            position: [newPoint.longitude, newPoint.latitude],
            address: newPoint.name,
            photos: [],
          },
        ])

        setTempMarker(null)
        setIsAddingMode(false)
      } catch (error) {
        console.error("Ошибка создания точки:", error)
        alert("Не удалось создать точку")
        setTempMarker(null)
      }
    }, 100)
  }

  const handleMarkerClick = async (marker) => {
    if (!marker.id) {
      alert("Ошибка: у выбранной точки нет ID")
      return
    }

    try {
      const pointDetails = await PhotoMapAPI.getPointDetails(marker.id)
      const locationWithId = {
        ...pointDetails,
        id: marker.id,
      }
      setSelectedLocation(locationWithId)
    } catch (error) {
      console.error("Ошибка загрузки данных точки:", error)
      setSelectedLocation({
        ...marker,
        id: marker.id,
      })
    }
  }

  // Компонент полноэкранного изображения
  const FullscreenPhoto = ({ photo, onClose }) => {
    const [isLiked, setIsLiked] = useState(likedPhotoIds[photo.id] || false)
    const [likesCount, setLikesCount] = useState(photo.likes || 0)

    useEffect(() => {
      const handleKeyPress = (e) => {
        if (e.key === "Escape") onClose()
      }
      document.addEventListener("keydown", handleKeyPress)
      return () => document.removeEventListener("keydown", handleKeyPress)
    }, [onClose])

    const handleLikeToggle = async () => {
      try {
        if (isLiked) {
          await PhotoMapAPI.dislikePhoto(photo.id)
          setLikesCount((prev) => prev - 1)
          setIsLiked(false)
          setLikedPhotoIds((prev) => {
            const newState = { ...prev }
            delete newState[photo.id]
            return newState
          })
        } else {
          await PhotoMapAPI.likePhoto(photo.id)
          setLikesCount((prev) => prev + 1)
          setIsLiked(true)
          setLikedPhotoIds((prev) => ({ ...prev, [photo.id]: true }))
        }
      } catch (error) {
        console.error("Ошибка при изменении лайка:", error)
      }
    }

    return (
        <div className="fullscreen-overlay" onClick={onClose}>
          <div className="fullscreen-content">
            <img
                src={photo.url || "/placeholder.svg"}
                alt="Полноэкранный просмотр"
                onClick={(e) => e.stopPropagation()}
                onError={(e) => {
                  console.error("Ошибка загрузки изображения:", photo.url)
                  e.target.src = "/placeholder.svg"
                }}
            />

            <div className="fullscreen-actions" onClick={(e) => e.stopPropagation()}>
              <button className="fullscreen-like-btn" onClick={handleLikeToggle}>
                <img src={isLiked ? "/like_full.png" : "/like.png"} alt="Лайк" className="heart-icon" />
                <span>{likesCount}</span>
              </button>
            </div>

            <button className="close-fullscreen-btn" onClick={onClose}>
              ×
            </button>
          </div>
        </div>
    )
  }

  // Построение маршрута
  const buildRoute = async () => {
    if (!selectedLocation || !routeStartAddress) {
      setRouteError("Укажите начальный адрес")
      return
    }

    try {
      const fullAddress = `${routeStartAddress}, Екатеринбург`

      const geocodeResponse = await axios.get(GEOCODE_URL, {
        params: {
          q: fullAddress,
          fields: "items.point",
          limit: 1,
          key: DGIS_API_KEY,
        },
      })

      const startItem = geocodeResponse.data.result?.items?.[0]
      if (!startItem) {
        setRouteError("Адрес не найден")
        return
      }

      const startPoint = startItem.point
      if (!startPoint) {
        setRouteError("Не удалось определить координаты для адреса")
        return
      }

      const routeParams = {
        points: [
          {
            type: "walking",
            lon: startPoint.lon,
            lat: startPoint.lat,
          },
          {
            type: "walking",
            lon: selectedLocation.position[0],
            lat: selectedLocation.position[1],
          },
        ],
        transport: "walking",
        route_mode: "fastest",
        traffic_mode: "jam",
        output: "detailed",
      }

      const routeResponse = await axios.post(`${DIRECTIONS_URL}?key=${DGIS_API_KEY}`, routeParams, {
        headers: {
          "Content-Type": "application/json",
        },
      })

      setShowRouteModal(false)
      clearRouteForm()

      if (routeResponse.data.result && routeResponse.data.result.length > 0) {
        const route = routeResponse.data.result[0]
        setRouteData(route)
        drawRouteOnMap(route)
        setRouteError("")
      } else {
        setRouteError("Не удалось построить маршрут")
      }
    } catch (error) {
      console.error("Ошибка построения маршрута:", error)
      setRouteError("Ошибка сервера при построении маршрута")
    }
  }

  const clearRouteForm = () => {
    setRouteStartAddress("")
    setRouteData(null)
  }

  const closeRouteModal = () => {
    setShowRouteModal(false)
    clearRouteForm()
  }

  const parseWKT = (wktString) => {
    if (!wktString) return []

    const match = wktString.match(/LINESTRING$$(.+)$$/)
    if (!match) return []

    const pointsStr = match[1]
    return pointsStr.split(",").map((point) => {
      const [lon, lat] = point.trim().split(/\s+/).map(Number.parseFloat)
      return [lon, lat]
    })
  }

  const clearRoute = () => {
    if (mapInstance.current?.route) {
      mapInstance.current.route.destroy()
      mapInstance.current.route = null
      setRouteDrawn(false)
    }
  }

  const drawRouteOnMap = (route) => {
    if (!mapInstance.current || !mapglAPI.current || !route) {
      return
    }

    clearRoute()

    const routeCoordinates = []

    if (route.maneuvers && Array.isArray(route.maneuvers)) {
      route.maneuvers.forEach((maneuver) => {
        if (maneuver.outcoming_path?.geometry) {
          const geometries = Array.isArray(maneuver.outcoming_path.geometry)
              ? maneuver.outcoming_path.geometry
              : [maneuver.outcoming_path.geometry]

          geometries.forEach((segment) => {
            if (segment?.selection) {
              const coords = parseWKT(segment.selection)
              routeCoordinates.push(...coords)
            }
          })
        }
      })
    }

    if (routeCoordinates.length > 0) {
      try {
        if (mapglAPI.current.Polyline) {
          const polyline = new mapglAPI.current.Polyline(mapInstance.current, {
            coordinates: routeCoordinates,
            color: "#1a21e0",
            width: 5,
          })

          mapInstance.current.route = polyline
          setRouteDrawn(true)
        }
      } catch (error) {
        console.error("Ошибка при рисовании маршрута:", error)
      }
    }
  }

  const removePhoto = (index) => {
    const newPhotosList = [...newPhotos]
    newPhotosList.splice(index, 1)
    setNewPhotos(newPhotosList)
  }

  const handleFileChange = (e) => {
    const files = Array.from(e.target.files)
    const validFiles = files.slice(0, 5).filter((file) => file.type.startsWith("image/"))
    setNewPhotos(validFiles)
  }

  const handleAddPhotos = async () => {
    if (newPhotos.length === 0) {
      alert("Выберите фотографии для загрузки")
      return
    }

    if (!selectedLocation?.id) {
      alert("Ошибка: у выбранной точки нет ID")
      return
    }

    if (!currentUser) {
      alert("Необходимо войти в систему")
      return
    }

    setIsUploading(true)

    try {
      const uploadedPhotos = await PhotoMapAPI.uploadMultiplePhotos(selectedLocation.id, newPhotos)

      const updatedLocation = {
        ...selectedLocation,
        photos: [...(selectedLocation.photos || []), ...uploadedPhotos],
      }

      setSelectedLocation(updatedLocation)

      setMarkers((prevMarkers) =>
          prevMarkers.map((marker) => (marker.id === selectedLocation.id ? updatedLocation : marker)),
      )

      setNewPhotos([])
      alert(`Успешно добавлено ${uploadedPhotos.length} фото!`)
    } catch (error) {
      console.error("Ошибка при добавлении фотографий:", error)
      alert(`Не удалось добавить фотографии: ${error.message || error}`)
    } finally {
      setIsUploading(false)
    }
  }

  const handleSaveMarker = async () => {
    const newPointData = {
      name: tempMarker.address,
      latitude: tempMarker.position[1],
      longitude: tempMarker.position[0],
      description: "Новая точка",
    }

    try {
      const createdPoint = await PhotoMapAPI.createPoint(newPointData)

      const newMarker = {
        ...tempMarker,
        id: createdPoint.id,
        photos: [],
      }

      const updatedMarkers = [...markers, newMarker]
      setMarkers(updatedMarkers)
      handleCancelAdding()
    } catch (error) {
      console.error("Ошибка создания точки:", error)
    }
  }

  return (
      <div className="map-container">
        <button onClick={toggleAddingMode} className={`mode-toggle ${isAddingMode ? "active" : ""}`}>
          {isAddingMode ? "Отменить добавление" : "Новая точка"}
        </button>

        {routeDrawn && (
            <button className="clear-route-button" onClick={clearRoute}>
              Очистить маршрут
            </button>
        )}

        <div ref={mapContainerRef} id="map-container" style={{ width: "100%", height: "800px", position: "relative" }}>
          {!mapLoaded && (
              <div className="map-loading">
                <p>Загрузка карты...</p>
              </div>
          )}
        </div>

        {/* Модальное окно просмотра точки */}
        <Modal
            isOpen={!!selectedLocation}
            onRequestClose={() => setSelectedLocation(null)}
            className="location-modal"
            overlayClassName="modal-overlay"
        >
          <div className="location-container">
            <button className="close-modal-btn" onClick={() => setSelectedLocation(null)}>
              ×
            </button>

            <div className="location-header">
              <h2>{selectedLocation?.address}</h2>
              <p className="photo-count">{selectedLocation?.photos?.length || 0} фотографий</p>
            </div>

            <div className="location-content">
              {/* Галерея фотографий */}
              {selectedLocation?.photos && selectedLocation.photos.length > 0 ? (
                  <div className="photos-gallery">
                    {selectedLocation.photos.map((photo, index) => (
                        <div key={photo.id} className="photo-item">
                          <img
                              src={photo.url || "/placeholder.svg"}
                              alt={`Фото ${index + 1}`}
                              onClick={() => setFullscreenPhoto(photo)}
                              onError={(e) => {
                                e.target.src = "/placeholder.svg"
                              }}
                          />
                          <div className="photo-likes">
                            <span>Лайки:</span>
                            <span>{photo.likes || 0}</span>
                          </div>
                        </div>
                    ))}
                  </div>
              ) : (
                  <div className="no-photos">
                    <p>Пока нет фотографий этого места</p>
                  </div>
              )}

              {/* Форма добавления фотографий */}
              <div className="add-photos-section">
                <h3>Добавить фотографии</h3>

                <div className="file-upload-area">
                  <label className="file-upload-label">
                    <input type="file" multiple accept="image/*" onChange={handleFileChange} className="file-input" />
                    <div className="upload-text">
                      <span>Выберите фотографии</span>
                      <small>До 5 файлов, JPG</small>
                    </div>
                  </label>
                </div>

                {newPhotos.length > 0 && (
                    <div className="photo-previews">
                      {newPhotos.map((photo, index) => (
                          <div key={index} className="preview-item">
                            <img src={URL.createObjectURL(photo) || "/placeholder.svg"} alt={`Preview ${index + 1}`} />
                            <button className="remove-preview-btn" onClick={() => removePhoto(index)}>
                              ×
                            </button>
                          </div>
                      ))}
                    </div>
                )}

                <div className="action-buttons">
                  <button
                      className="add-photos-btn"
                      onClick={handleAddPhotos}
                      disabled={isUploading || newPhotos.length === 0}
                  >
                    {isUploading ? "Загрузка..." : `Добавить ${newPhotos.length} фото`}
                  </button>

                  <button className="route-btn" onClick={() => setShowRouteModal(true)}>
                    Маршрут
                  </button>
                </div>
              </div>
            </div>
          </div>
        </Modal>

        {/* Модальное окно построения маршрута */}
        <Modal
            isOpen={showRouteModal}
            onRequestClose={closeRouteModal}
            className="route-modal"
            overlayClassName="modal-overlay"
        >
          <div className="route-form">
            <h3>Построить маршрут</h3>
            <div className="route-input">
              <label>Откуда:</label>
              <input
                  type="text"
                  value={routeStartAddress}
                  onChange={(e) => setRouteStartAddress(e.target.value)}
                  placeholder="Введите начальный адрес"
              />
            </div>

            {routeError && <div className="route-error">{routeError}</div>}

            <div className="route-buttons">
              <button className="confirm-btn" onClick={buildRoute}>
                Построить
              </button>
              <button onClick={closeRouteModal}>Закрыть</button>
            </div>
          </div>
        </Modal>

        {/* Полноэкранное изображение */}
        {fullscreenPhoto && <FullscreenPhoto photo={fullscreenPhoto} onClose={() => setFullscreenPhoto(null)} />}

        {/* Модальное окно создания новой метки */}
        <Modal
            isOpen={!!tempMarker}
            onRequestClose={handleCancelAdding}
            className="new-marker-modal"
            overlayClassName="modal-overlay"
        >
          {tempMarker && (
              <div className="new-marker-form">
                <h2>Новая метка: {tempMarker.address}</h2>
                <div className="marker-buttons">
                  <button onClick={handleSaveMarker}>Сохранить метку</button>
                  <button onClick={() => setTempMarker(null)}>Отмена</button>
                </div>
              </div>
          )}
        </Modal>
      </div>
  )
}

export default MapWithControls
