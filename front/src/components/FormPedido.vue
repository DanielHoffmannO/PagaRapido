<template>
  <div class="form-card">
    <h2>Novo Pedido</h2>
    <form @submit.prevent="criarPedido">
      <div class="field">
        <label>Nome</label>
        <input v-model="form.clienteNome" required placeholder="Seu nome" />
      </div>
      <div class="field">
        <label>Email</label>
        <input v-model="form.clienteEmail" type="email" required placeholder="email@exemplo.com" />
      </div>

      <h3>Itens</h3>
      <div v-for="(item, i) in form.itens" :key="i" class="item-row">
        <input v-model="item.produto" placeholder="Produto" required />
        <input v-model.number="item.quantidade" type="number" min="1" placeholder="Qtd" required />
        <input v-model.number="item.precoUnitario" type="number" step="0.01" min="0.01" placeholder="Preço" required />
        <button type="button" class="btn-remove" @click="removerItem(i)" v-if="form.itens.length > 1">✕</button>
      </div>
      <button type="button" class="btn-secondary" @click="adicionarItem">+ Adicionar item</button>

      <button type="submit" class="btn-primary" :disabled="loading">
        {{ loading ? 'Criando...' : 'Criar Pedido (Pix)' }}
      </button>
    </form>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import api from '../api'

const emit = defineEmits(['pedido-criado'])

const loading = ref(false)
const form = reactive({
  clienteNome: '',
  clienteEmail: '',
  itens: [{ produto: '', quantidade: 1, precoUnitario: 0 }]
})

function adicionarItem() {
  form.itens.push({ produto: '', quantidade: 1, precoUnitario: 0 })
}

function removerItem(i) {
  form.itens.splice(i, 1)
}

async function criarPedido() {
  loading.value = true
  try {
    const { data } = await api.post('/api/pedidos', {
      clienteNome: form.clienteNome,
      clienteEmail: form.clienteEmail,
      itens: form.itens
    })
    emit('pedido-criado', data)
    form.clienteNome = ''
    form.clienteEmail = ''
    form.itens = [{ produto: '', quantidade: 1, precoUnitario: 0 }]
  } catch (e) {
    alert('Erro ao criar pedido: ' + (e.response?.data || e.message))
  } finally {
    loading.value = false
  }
}
</script>
