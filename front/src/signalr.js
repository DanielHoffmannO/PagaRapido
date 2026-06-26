import * as signalR from '@microsoft/signalr'

const hubUrl = import.meta.env.VITE_API_URL || 'http://localhost:5000'

const connection = new signalR.HubConnectionBuilder()
  .withUrl(`${hubUrl}/hubs/pedido`)
  .withAutomaticReconnect()
  .build()

export async function startConnection() {
  if (connection.state === signalR.HubConnectionState.Disconnected) {
    await connection.start()
  }
}

export function acompanharPedido(pedidoId) {
  return connection.invoke('AcompanharPedido', pedidoId)
}

export function onPagamentoConfirmado(callback) {
  connection.on('PagamentoConfirmado', callback)
}

export function offPagamentoConfirmado(callback) {
  connection.off('PagamentoConfirmado', callback)
}

export default connection
