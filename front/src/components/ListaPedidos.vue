<template>
  <div class="list-card">
    <h2>Pedidos</h2>
    <p v-if="!pedidos.length" class="empty">Nenhum pedido ainda.</p>
    <div v-for="p in pedidos" :key="p.id" class="pedido-item" :class="statusClass(p.status)">
      <div class="pedido-header">
        <strong>{{ p.clienteNome }}</strong>
        <span class="badge">{{ statusLabel(p.status) }}</span>
      </div>
      <div class="pedido-body">
        <span>{{ p.itens.length }} item(ns)</span>
        <span class="total">R$ {{ calcTotal(p.itens).toFixed(2) }}</span>
      </div>
      <small class="pedido-id">{{ p.id }}</small>
    </div>
  </div>
</template>

<script setup>
defineProps({ pedidos: Array })

function statusLabel(s) {
  const map = { 1: 'Criado', 2: 'Aguardando Pix', 3: '✅ Pago', 4: 'Cancelado' }
  return map[s] || s
}

function statusClass(s) {
  return { 1: 'status-criado', 2: 'status-aguardando', 3: 'status-pago', 4: 'status-cancelado' }[s]
}

function calcTotal(itens) {
  return itens.reduce((acc, i) => acc + i.quantidade * i.precoUnitario, 0)
}
</script>
