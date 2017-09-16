function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    };
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
}

function add_message(message, status) {
    var ui_style, ui_icon;
    switch (status) {
        case "success":
            ui_style = "positive";
            ui_icon = "check circle outline";
            break;
        case "warning":
            ui_style = "warning";
            ui_icon = "warning circle";
            break;
        case "error":
            ui_style = "negative";
            ui_icon = "minus circle";
            break;
        default:
            ui_style = "info";
            ui_icon = "info circle";
            break;
    };

    var msg_area = $("#notification-area");
    var msg_box = $("#template-notification").clone();

    msg_box.attr("id", "notification-" + guid());
    msg_box.attr("class", "ui icon " + ui_style + " message");
    msg_box.prepend($('<i class="' + ui_icon + ' icon"></i>'));
    msg_box.children("div.content").children("div.header").text(message);
    msg_box.children("div.content").children("p").text(moment().format('LLLL'));

    msg_box.children("i.close").on('click', function () { $(this).closest('.message').transition('fade'); });

    msg_area.prepend(msg_box);
};

$(function () {
    moment.locale('zh-cn');
    moment.updateLocale('zh-cn', {
        longDateFormat: {
            LLLL: "YYYY年MMMD日 dddd Ah点mm分ss秒",
        }
    });

    var chat = $.connection.reportHub;

    chat.client.addMessage = function (message, type) {
        add_message(message, type);
    };

    chat.client.updateFocusIndex = function (number) {
        $("#number-focus-index").text(number);
    };

    chat.client.updateHandupCount = function (number) {
        $("#number-handup-count").text(number);
    };

    $.connection.hub.start();
});
