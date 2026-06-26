<template>
  <div id="app">
    <header>
      <h1>💸 PagaRápido</h1>
      <p>Pagamento Pix em tempo real</p>
    </header>

    <Notificacao @pagamento-confirmado="onPagamento" />

    <main>
      <FormPedido @pedido-criado="onPedidoCriado" />
      <ListaPedidos :pedidos="pedidos" />
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from './api'
import { acompanharPedido } from './signalr'
import FormPedido from './components/FormPedido.vue'
import ListaPedidos from './components/ListaPedidos.vue'
import Notificacao from './components/Notificacao.vue'

const pedidos = ref([])

onMounted(async () => {
  const { data } = await api.get('/api/pedidos')
  pedidos.value = data
  data.forEach(p => { if (p.status < 3) acompanharPedido(p.id) })
})

function onPedidoCriado(pedido) {
  pedidos.value.unshift(pedido)
  acompanharPedido(pedido.id)
}

function onPagamento({ pedidoId }) {
  const p = pedidos.value.find(x => x.id === pedidoId)
  if (p) p.status = 3
}
</script>
