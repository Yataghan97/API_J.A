// src/services/usuarioService.js
import api from './api';

const API_URL = '/usuarios';

export const getUsuariosPendentes = async () => {
  const response = await api.get(`${API_URL}/pendentes`);
  return response.data;
};

export const aprovarUsuario = async (id) => {
  const response = await api.put(`${API_URL}/${id}/aprovar`);
  return response.data;
};
