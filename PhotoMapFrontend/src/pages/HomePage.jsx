"use client"

import { Link } from "react-router-dom"
import { useAuth } from "../AuthContext"
import MapWithControls from "../MapWithControls"

export default function HomePage() {
    const { currentUser, logout } = useAuth()

    return (
        <div className="home-page">
            <header className="home-header">
                <div className="header-content">
                    <div className="logo-section">
                        <img className="header-logo" src="/pen.png" alt="PhotoFinder" />
                        <span className="app-title">PhotoFinder</span>
                    </div>

                    <div className="header-actions">
                        <Link className="profile-link" to="/account">
                            <span>{currentUser?.userName || "Профиль"}</span>
                        </Link>
                        <button className="logout-btn" onClick={logout}>
                            Выход
                        </button>
                    </div>
                </div>
            </header>

            <main className="home-main">
                <MapWithControls />
            </main>
        </div>
    )
}
