$(function () {
   
    $(".cart-qty").change(function () {       
        Update($(this).data("productid"), $(this).val());       
    });
        
});


function Update(productId, value) {

    var url;

    if (productId > 0 && value > -1) {
        url = "/Cart/" + productId + "/" + value;
    }
    else {
        url = ""
    }
    $.post(url, function (value) {
        parseAmount(value["cost"], "#total-" + productId);
        parseAmount(value["total"], "#cart-total");
    });
}

function parseAmount(value, targetField) {
    $amount = $(targetField);

    // Clear the previous options
    $($amount).empty();
    $($amount).html(value);
  
}
