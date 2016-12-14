$(document).ready(function() {
    starRating.create('.stars');
});

// The widget
var starRating = {
    create: function(selector) {
        // loop over every element matching the selector
        $(selector).each(function() {
            var $list = $('<span></span>');
            // loop over every radio button in each container
            $(this)
        .find('input:radio')
        .each(function(i) {
            var rating = $(this).val();
            var $item = $('<span></span>')
            .attr('title', rating)
            .text(rating);

            if (!$(this).closest(selector).hasClass('readonly'))
            {
                starRating.addHandlers($item);
            }
            $list.append($item);

            if ($(this).is(':checked')) {
                $item.prevAll().andSelf().addClass('rating');
            }
        });
            // Hide the original radio buttons
            $(this).append($list).find('label').hide();
        });
    },
    addHandlers: function(item) {
        $(item).click(function(e) {
            // Handle Star click
            var $star = $(this);
            var $allLinks = $(this).parent();

            // Set the radio button value
            $allLinks
            .parent()
            .find('input:radio[value=' + $star.text() + ']')
            .attr('checked', true);

            // Set the ratings
            $allLinks.children().removeClass('rating');
            $star.prevAll().andSelf().addClass('rating');

            // prevent default link click
            e.preventDefault();
            
            // submit the form

        }).hover(function() {
            // Handle star mouse over
            $(this).prevAll().andSelf().addClass('rating-over');
        }, function() {
            // Handle star mouse out
            $(this).siblings().andSelf().removeClass('rating-over')
        });
    }
}