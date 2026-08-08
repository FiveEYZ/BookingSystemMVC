// booking.js - simple client side SignalR + helper functions

window.bookingInit = async function(serviceId, date, onUpdate) {
    if (!window.signalR) {
        // Simple runtime check - ensure SignalR script is loaded by layout
        console.warn('SignalR not loaded. Real-time updates disabled.');
        return;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl('/hubs/booking')
        .withAutomaticReconnect()
        .build();

    connection.on('SlotReserved', payload => {
        // payload: { serviceId, startDateTime, reservationId }
        const d = new Date(payload.startDateTime);
        // notify and refresh
        onUpdate();
    });

    connection.on('SlotReleased', payload => onUpdate());
    connection.on('SlotBooked', payload => onUpdate());

    try {
        await connection.start();
        window._bookingConnection = connection;
        // subscribe to initial
        await window.bookingSubscribe(serviceId, date);
    } catch (e) {
        console.error('SignalR connect failed', e);
    }
};

window.bookingSubscribe = async function(serviceId, date) {
    if (!window._bookingConnection) return;
    try {
        await window._bookingConnection.invoke('Subscribe', serviceId, date);
    } catch (e) {
        console.error('Subscribe failed', e);
    }
};
