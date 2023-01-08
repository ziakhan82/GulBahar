redirectToCheckout = function (sessionId) {
    var stripe = Stripe("pk_test_51MKRgtBDQzLgw4KEf75JBNzFjk1uq8BKODAQEUYVtKYkvuljYNhNNtRTUFtWJOOael1dXqgDi0JdwCMlKuKIDknT00IN15Evu9");
    stripe.redirectToCheckout({ sessionId: sessionId });
}