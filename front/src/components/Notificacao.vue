<template>
  <transition name="fade">
    <div v-if="notificacao" class="toast">
      ✅ Pagamento confirmado! Pedido {{ notificacao.pedidoId.slice(0, 8) }}... — R$ {{ notificacao.valor.toFixed(2) }}
    </div>
  </transition>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { startConnection, onPagamentoConfirmado, offPagamentoConfirmado } from '../signalr'

const emit = defineEmits(['pagamento-confirmado'])
const notificacao = ref(null)
let timeout = null

function handler(data) {
  notificacao.value = data
  emit('pagamento-confirmado', data)
  clearTimeout(timeout)
  timeout = setTimeout(() => { notificacao.value = null }, 5000)
}

onMounted(async () => {
  await startConnection()
  onPagamentoConfirmado(handler)
})

onUnmounted(() => {
  offPagamentoConfirmado(handler)
})
</script>
