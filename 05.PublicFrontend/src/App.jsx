import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import MainLayout from './layouts/MainLayout';
import ProtectedRoute from './components/ProtectedRoute';
import Home from './pages/Home';
import Products from './pages/Products';
import ProductDetail from './pages/ProductDetail';
import Cart from './pages/Cart';
import Checkout from './pages/Checkout';
import Orders from './pages/Orders';
import Login from './pages/Login';
import Register from './pages/Register';

import { CartProvider } from './context/CartContext';

function App() {
    return (
        <CartProvider>
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<MainLayout />}>
                        <Route index element={<Home />} />
                        <Route path="products" element={<Products />} />
                        <Route path="products/:id" element={<ProductDetail />} />
                        <Route path="cart" element={<Cart />} />
                        <Route path="login" element={<Login />} />
                        <Route path="register" element={<Register />} />

                        <Route element={<ProtectedRoute />}>
                            <Route path="checkout" element={<Checkout />} />
                            <Route path="orders" element={<Orders />} />
                        </Route>

                        {/* Catch all */}
                        <Route path="*" element={<div style={{ textAlign: 'center', marginTop: '4rem' }}><h2>404 Page Not Found</h2></div>} />
                    </Route>
                </Routes>
            </BrowserRouter>
        </CartProvider>
    );
}

export default App;
