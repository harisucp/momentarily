﻿angular
    .module('MomentarilyApp')
    .controller('MessageController', MessageControllerobj)
    .controller('ConversationController', ConversationControllerobj);

MessageControllerobj.$inject = ['$http', '$window', 'clrDateTime', 'MessageService', 'Messages'];
function MessageControllerobj($http, $window, clrDateTime, MessageService, Messages) {
    
    var vm = this;

    vm.convertDate = clrDateTime.convertToDate;
    vm.showMessagesList = showMessagesList;
    vm.isMessageNew = isMessageNew;
    vm.replaceQuotes = replaceQuotes;

    setValues();

    function setValues() {
        vm.viewModel = Messages.ViewModel;
        vm.messages = vm.viewModel.Messages;
        vm.receiverUserName = Messages.ViewModel.ReceiverUserName;
    }

    function showMessagesList() {
        return vm.viewModel.length !== 0;
    }

    function isMessageNew(message) {
        return !message.IsRead;
    }
    function replaceQuotes(html) {
        return html.replace(/\\\"/g, '\"');
    }
}

ConversationControllerobj.$inject = ['$http', '$window', '$sce', '$timeout', '$rootScope', '$filter', 'clrDateTime', 'MessageService', 'Messages'];
function ConversationControllerobj($http, $window, $sce, $timeout, $rootScope, $filter, clrDateTime, MessageService, Messages) {
    
    var vm = this;

    vm.viewModel = Messages.ViewModel;
    vm.convertDate = clrDateTime.convertToDate;
    vm.isAuthorsMessage = isAuthorsMessage;
    vm.submitForm = submitForm;
    vm.timeForReadMessagesTimeOut = 3000;
    vm.replaceQuotes = replaceQuotes;
    vm.trustedHtml = trustedHtml;
    vm.CurrentDate = moment(new Date());
    vm.calendarFormats = {
        sameDay: '[Today ]MM.DD.YYYY',
        nextDay: 'MM.DD.YYYY',
        nextWeek: 'MM.DD.YYYY',
        lastDay: '[Yesterday ]MM.DD.YYYY',
        lastWeek: 'MM.DD.YYYY',
        sameElse: 'MM.DD.YYYY'
    };

    (function () {
        setValues();
        setIsReadMessages();
    })();

    function setValues() {
        vm.propertyName = 'DateCreated';

        vm.viewModel.Messages = vm.viewModel.Messages || [];

        vm.messages = $filter('orderBy')(vm.viewModel.Messages, vm.propertyName);
        vm.groupedMessages = [];
        if (vm.messages.length !== 0) {
            vm.groupedMessages.push({
                date: moment(vm.messages[0].DateCreated),
                messages: []
            });
            for (var i = 0; i < vm.messages.length; i++) {
                if (!moment(vm.messages[i].DateCreated).isSame(vm.groupedMessages[vm.groupedMessages.length - 1].date, 'day')) {
                    vm.groupedMessages.push({
                        date: moment(vm.messages[i].DateCreated),
                        messages: []
                    });
                }
                if (moment(vm.messages[i].DateCreated).isSame(vm.groupedMessages[vm.groupedMessages.length - 1].date, 'day')) {
                    vm.groupedMessages[vm.groupedMessages.length - 1].messages.push(vm.messages[i]);
                }
            }
        }
        vm.receiverUserName = Messages.ViewModel.ReceiverUserName;
        vm.isConversationListEmpty = (vm.messages.length === 0);
    }

    function isAuthorsMessage(message) {
        return message.AuthorId === vm.viewModel.AuthorId;
    }

    function submitForm($event, form) {
        if (form.$invalid) {
            $event.preventDefault();
            form.$submitted = true;
        } else {
            var receiverId = vm.viewModel.ReceiverId;
            var data = {
                "AuthorId": vm.viewModel.AuthorId,
                "ReceiverId": receiverId,
                "Message": protectFromQuotes(vm.message)
            };

            MessageService.PostMessage(data).then(function () {
                $window.location = "/Message/Conversation?userId=" + receiverId;
            }, function (response) {
                console.log("trouble\n" + response);
            });
        }
    }

    function trustedHtml(html) {
        return $sce.trustAsHtml($filter('newlines')(html));
    }

    function protectFromQuotes(string) {
        return string.replace(/\"/g, '\\\"');
    }

    function replaceQuotes(html) {
        return html.replace(/\\\"/g, '\"');
    }

    function setIsReadMessages() {
        if (vm.messages) {
            for (var i = 0; i < vm.messages.length; i++) {
                if (!vm.messages[i].IsRead) {
                    $timeout(readMessage, vm.timeForReadMessagesTimeOut);
                    break;
                }
            }
        }
    }

    function readMessage() {
        var data = {
            "AuthorId": vm.viewModel.ReceiverId
        }

        MessageService.ReadMessage(data).then(function () {

            $rootScope.$broadcast('reload-userdata');

        }, function (response) {
            console.log("trouble\n" + JSON.parse(response));
        });
    }
}