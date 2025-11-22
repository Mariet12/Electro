'use client';

import React, { createContext, useContext, useState, useEffect, ReactNode, useCallback, useMemo } from 'react';
import api from '@/lib/api';
import toast from 'react-hot-toast';

interface CartItem {
  id: number;
  productId: number;
  productName: string;
  productImage: string;
  quantity: number;
  price: number;
  totalPrice: number;
}

interface Cart {
  items: CartItem[];
  totalAmount: number;
  itemsCount: number;
}

interface CartContextType {
  cart: Cart | null;
  loading: boolean;
  addToCart: (productId: number, quantity: number) => Promise<void>;
  updateQuantity: (cartItemId: number, quantity: number) => Promise<void>;
  removeItem: (cartItemId: number) => Promise<void>;
  clearCart: () => Promise<void>;
  refreshCart: () => Promise<void>;
}

const CartContext = createContext<CartContextType | undefined>(undefined);

export const CartProvider = ({ children }: { children: ReactNode }) => {
  const [cart, setCart] = useState<Cart | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      refreshCart();
    }
  }, []);

  const refreshCart = useCallback(async () => {
    try {
      const response = await api.get('/cart');
      setCart(response.data.data);
    } catch (error) {
      console.error('Error fetching cart:', error);
    }
  }, []);

  const addToCart = useCallback(async (productId: number, quantity: number) => {
    setLoading(true);
    try {
      const response = await api.post('/cart/add', { productId, quantity });
      setCart(response.data.data);
      toast.success('تم إضافة المنتج للسلة');
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'حدث خطأ');
    } finally {
      setLoading(false);
    }
  }, []);

  const updateQuantity = useCallback(async (cartItemId: number, quantity: number) => {
    setLoading(true);
    try {
      const response = await api.put('/cart/items', { cartItemId, quantity });
      setCart(response.data.data);
      toast.success('تم تحديث الكمية');
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'حدث خطأ');
    } finally {
      setLoading(false);
    }
  }, []);

  const removeItem = useCallback(async (cartItemId: number) => {
    setLoading(true);
    try {
      await api.delete(`/cart/items/${cartItemId}`);
      await refreshCart();
      toast.success('تم حذف المنتج من السلة');
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'حدث خطأ');
    } finally {
      setLoading(false);
    }
  }, [refreshCart]);

  const clearCart = useCallback(async () => {
    setLoading(true);
    try {
      await api.delete('/cart');
      setCart({ items: [], totalAmount: 0, itemsCount: 0 });
      toast.success('تم تفريغ السلة');
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'حدث خطأ');
    } finally {
      setLoading(false);
    }
  }, []);

  const value = useMemo(() => ({
    cart,
    loading,
    addToCart,
    updateQuantity,
    removeItem,
    clearCart,
    refreshCart,
  }), [cart, loading, addToCart, updateQuantity, removeItem, clearCart, refreshCart]);

  return (
    <CartContext.Provider value={value}>
      {children}
    </CartContext.Provider>
  );
};

export const useCart = () => {
  const context = useContext(CartContext);
  if (context === undefined) {
    throw new Error('useCart must be used within a CartProvider');
  }
  return context;
};

