import React from 'react';
import { BrowserRouter, Routes, Route } from "react-router-dom";

import { AuthContextProvider } from './context/AuthContext';
import { BasketProvider } from './context/BasketContext';

import Home from './components/pages/Home';
import Login from './components/pages/Login';
import Basket from './components/pages/Basket';
import Category from './components/pages/Category';
import NoPage from './components/pages/NoPage';

import './App.css';
import { Checkout } from './components/pages/Checkout';


const App = () => {
  return (
    <AuthContextProvider>
      <BasketProvider>
        <BrowserRouter>
          <Routes>
            <Route index element={<Home />} />
            <Route path="category/*" element={<Category />} />              
            <Route path="basket" element={<Basket />} />
            <Route path="checkout" element={<Checkout />} />
            <Route path="login" element={<Login />} />
            <Route path="*" element={<NoPage />} />
          </Routes>
        </BrowserRouter>
      </BasketProvider>
    </AuthContextProvider>
  );
}

export default App;
