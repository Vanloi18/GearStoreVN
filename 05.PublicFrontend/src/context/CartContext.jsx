import React, { createContext, useContext, useState, useEffect } from 'react';

const CartContext = createContext();

export const useCart = () => useContext(CartContext);

export const CartProvider = ({ children }) => {
    const [cartItems, setCartItems] = useState(() => {
        const savedCart = localStorage.getItem('cart');
        return savedCart ? JSON.parse(savedCart) : [];
    });

    useEffect(() => {
        localStorage.setItem('cart', JSON.stringify(cartItems));
    }, [cartItems]);

    const getCartItemKey = (productId, variantId) => {
        return `${productId}-${variantId || 'base'}`;
    };

    const addToCart = (product, quantity = 1, variantId = null, variantName = null, price = null) => {
        setCartItems(prev => {
            const itemKey = getCartItemKey(product.id, variantId);
            const existing = prev.find(item => getCartItemKey(item.id, item.variantId) === itemKey);

            if (existing) {
                return prev.map(item =>
                    getCartItemKey(item.id, item.variantId) === itemKey
                        ? { ...item, quantity: item.quantity + quantity }
                        : item
                );
            }

            return [...prev, {
                ...product,
                id: product.id,
                variantId,
                variantName,
                price: price || product.price, // Use variant price if provided
                quantity
            }];
        });
    };

    const removeFromCart = (productId, variantId = null) => {
        setCartItems(prev => prev.filter(item => getCartItemKey(item.id, item.variantId) !== getCartItemKey(productId, variantId)));
    };

    const updateQuantity = (productId, newQuantity, variantId = null) => {
        if (newQuantity < 1) return;
        setCartItems(prev =>
            prev.map(item =>
                getCartItemKey(item.id, item.variantId) === getCartItemKey(productId, variantId)
                    ? { ...item, quantity: newQuantity }
                    : item
            )
        );
    };

    const clearCart = () => {
        setCartItems([]);
    };

    const cartCount = cartItems.reduce((acc, item) => acc + item.quantity, 0);
    const cartTotal = cartItems.reduce((acc, item) => acc + item.price * item.quantity, 0);

    return (
        <CartContext.Provider value={{
            cartItems,
            addToCart,
            removeFromCart,
            updateQuantity,
            clearCart,
            cartCount,
            cartTotal
        }}>
            {children}
        </CartContext.Provider>
    );
};
