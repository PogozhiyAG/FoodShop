// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    function applyBasketData(basketData) {
        $('.basket__total-amount').text(basketData.totalAmount);

        $('.product__item').each((i, pi) => {
            const productId = $(pi).data('productId');
            const productData = basketData.items.find(i => i.productId == productId);

            $(pi).find('.basket__item-quantity').text(productData ? productData.quantity : '');
        });
    }

    $('.product__item .btn-action-add').click(e => {
        const $product_item = $(e.target).closest('.product__item');
        const productId = $product_item.data('productId');
        $.post(`/Basket/AddBasketItem?productId=${productId}&quantity=1`, function (data) {
            applyBasketData(data);
        });
    });
    $('.product__item .btn-action-remove').click(e => {
        const $product_item = $(e.target).closest('.product__item');
        const productId = $product_item.data('productId');
        $.post(`/Basket/AddBasketItem?productId=${productId}&quantity=-1`, function (data) {
            applyBasketData(data);
        });
    });


    $.get('/Basket/GetBasket', function (data) {
        applyBasketData(data);
    });
});