import { useState } from 'react';
import api from '../services/api';

export default function RegisterPage() {
  const [form, setForm] = useState({ nome: '', email: '', senha: '' });

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    try {
      await api.post('/usuarios', form);
      alert('Cadastro enviado para aprovação!');
    } catch (error) {
      alert('Erro ao cadastrar.');
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <form className="bg-white p-8 rounded shadow-md" onSubmit={handleRegister}>
        <h2 className="text-xl mb-4">Cadastro</h2>
        <input name="nome" placeholder="Nome" className="w-full mb-2 p-2 border" onChange={handleChange} />
        <input name="email" placeholder="Email" className="w-full mb-2 p-2 border" onChange={handleChange} />
        <input name="senha" placeholder="Senha" type="password" className="w-full mb-4 p-2 border" onChange={handleChange} />
        <button className="bg-green-600 text-white w-full p-2 rounded">Cadastrar</button>
      </form>
    </div>
  );
}
