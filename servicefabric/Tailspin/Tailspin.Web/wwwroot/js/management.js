$(document).ready(function () {
    $('.subscriptionSelector').change(function () {
        $('.standardConfigSelected').toggle();
        $('.premiumConfig').toggle('slow');
    });

    if ($('.subscriptionSelector').val() == 'Standard') {
        $('.premiumConfig').hide();
        $('.standardConfigSelected').show();
    }
});
