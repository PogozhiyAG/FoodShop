import React from 'react';
import { BrowserRouter, Routes, Route } from "react-router-dom";

import { AuthContextProvider } from './context/AuthContext';
import { BasketProvider } from './context/BasketContext';

import Home from './Home';
import Login from './Login';
import Basket from './Basket';
import Category from './Category';
import NoPage from './NoPage';

import './App.css';


const App = () => {
  return (
    <AuthContextProvider>
      <BasketProvider>
        <BrowserRouter>
          <Routes>
            <Route index element={<Home />} />
            <Route path="category/*" element={<Category />} />              
            <Route path="basket" element={<Basket />} />
            <Route path="login" element={<Login />} />
            <Route path="*" element={<NoPage />} />
          </Routes>
        </BrowserRouter>
      </BasketProvider>
    </AuthContextProvider>
  );
}

export default App;
