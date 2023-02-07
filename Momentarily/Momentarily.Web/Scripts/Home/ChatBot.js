jQuery(document).ready(function() {
	var error = {};
   var ActiveCustomerInfo = {};
   var BusinessName = '';
   var CurrentURL = jQuery('#script').attr('src');
   var url = new URL(CurrentURL);
   ClientURL = url.origin;
   var APIKey = url.searchParams.get("APIKey");
   jQuery('body').append("<button class='chatbox-open'><i class='fa fa-comment fa-2x' aria-hidden='true'></i></button> <button class='chatbox-close'> <i class='fa fa-close fa-2x' aria-hidden='true'></i></button> <section class='chatbox-popup'> <header class='chatbox-popup__header'> <aside style='flex:8'> <h2>"+ BusinessName +"</h2> </aside> <aside style='flex:1'> <button class='chatbox-maximize chat-reset'><i class='fa fa-refresh' aria-hidden='true'></i></button> </aside> </header> <main class='chatbox-popup__main'> <div id='chat'></div> <div id='loading'><div id='loading-center-absolute'><div class='object' id='object_one'></div><div class='object' id='object_two'></div><div class='object' id='object_three'></div></div></div> </main> </section>");

   var css_link = jQuery("<link>", { 
       rel: "stylesheet", 
       type: "text/css", 
       href: ClientURL+"/ChatBot.css" 
   });
   css_link.appendTo('head');

   jQuery.ajaxSetup({
      url: ClientURL+"/tlapi.php",
      data: { 'key' : APIKey }
   });

   jQuery('.chatbox-open').click(function(){
      jQuery(".chatbox-popup, .chatbox-close").fadeIn();
      OpenChatBot();
   });

   jQuery('.chatbox-close').click(function(){
      jQuery(".chatbox-popup, .chatbox-close").fadeOut();
   });

   jQuery('.chat-reset').click(function(){
      OpenChatBot();
   });

   getTLOptionValue('BusinessName', 'header');
});

function OpenChatBot() {
   jQuery('#chat').html('');
   var error = {};
   var ActiveCustomerInfo = {};
   ClientConversation('StartConversation');
}

function ClientConversation(BotTag, OnlinePaymentLink = '') {
   jQuery('#loading').show();
   var BusinessName = '';
   
   jQuery('.TLBotCustomerServiceMessage').remove();
   if(BotTag == 'TLBotCustomerServiceMessage')
   {
      jQuery('#loading').hide();
      getTLOptionValue("TLBotCustomerServiceMessage", 'TLBotCustomerServiceMessage');
      return false;
   }
   setTimeout(function() {
      jQuery.ajax({ 
         method: "GET",
         headers: {
             "content-type": "application/x-www-form-urlencoded"
         },
         data: {
            "Action": "api",
            "fn": "getChatMessages",
            "BotTag": BotTag
         },
         success: function(response) {
            var response = JSON.parse(response);
            if(response.Status == 0) 
            {
               jQuery('#loading').hide();
               response.Message = "<p>We were unable to find your information. Please check name and phone number and try again.</p>";
               ChatHandler(response, 'left');
            } 
            else 
            {
               jQuery('#loading').hide();
               if(response.Action != undefined && response.Action == 'GetFreeText' || response.Action == 'PerformTask'  || response.Action == 'GetOptions' || response.Action == 'ChooseAccountNoFoundOptions') 
               {
                  eval(response.Action+"("+JSON.stringify(response)+")");
               } 
               else 
               {
                  response.Message = response.Message.replace(/{OnlinePaymentLink}/g, OnlinePaymentLink);
                  ChatHandler(response, 'left '+BotTag);
               }
            }  
         }
      });
   }, 1000);
}

function ChatHandler(response, className) {
   jQuery('#chat').append("<div class='message "+className+' '+response.BotTag +"'>"+ response.Message +"</div>");

   if(response.NextBotTag == 'AskFirstLastName' || response.NextBotTag == 'GetFirstLastName' || response.NextBotTag == 'ChooseAccountNoFoundOptions')
   {
      jQuery('.customerinfo , .verifyphone, .VerificationCodeSent').remove();
      ClientConversation(response.NextBotTag);
   }
   scrollToBottom();
}

function scrollToBottom() {
   jQuery(".chatbox-popup__main").scrollTop(jQuery(".chatbox-popup__main").innerHeight() + 5000); 
}

function GetFreeText(response) {
	jQuery(".AccountNotFound, .try-again, .TLBotCustomerServiceMessage, .InvalidPhoneNumber, .TextCodeNotVerified").remove();
	jQuery('#chat').append("<div class='message right customerinfo'><form id='myform'><div class='message right'> <form name='contact' action=' id='customerForm'><div class='row'><div class='col-xs-6 padLeft-0'> <div class='form-group'><input type='text' class='form-control' name='firtname' id='firtname' placeholder='First Name' onkeyup='EnableButton()'></div></div><div class='col-xs-6 padRight-0'><div class='form-group'><input type='text' class='form-control' name='lastname' id='lastname' placeholder='Last Name' onkeyup='EnableButton()'></div></div><div class='col-xs-6 padLeft-0'><div class='form-group'><input type='text' class='form-control' name='phonenumber' id='phonenumber' placeholder='Phone Number' onkeyup='EnableButton()'></div></div><div class='col-xs-6 padRight-0'><div class='form-group'><button type='button' class='btn btn-success' onClick='ClientConversation(\"VerifyCustomer\")' id='submit' disabled> Verify Account <i class='fa fa-paper-plane' aria-hidden='true'></i></button></div></div></div></form></div></form></div>");
   scrollToBottom();
}

function ChooseAccountNoFoundOptions(response) {
	var buttons = '';
	jQuery.each(JSON.parse(response.Options), function( key, value ) {
	  buttons += "<button type='button' class='message message-kinda-tight my-message clickable flex-item flex-item-kinda-space-left' onclick='ClientConversation(\""+value+"\");'>"+ key +"</button>";
	});

   jQuery("#loading").hide(); 
   response.Message = buttons;
   ChatHandler(response, "right message-button try-again");
}

function GetOptions(response) {
	var buttons = '';

	jQuery.each(JSON.parse(response.Options), function( key, value ) {
	    buttons += "<button type='button' class='message message-kinda-tight my-message clickable flex-item' onclick='TLBotAction(\""+value+"\");'>"+ key +"</button>";
	});
	jQuery('#chat').append("<div class='message-button tl-actions'>"+buttons+"</div>");
   scrollToBottom();
}

function TLBotAction(action) {
   jQuery(".tl-actions button").attr('disabled', 'disabled');
   if(action == 'CancelAccount')
   {
      if(ActiveCustomerInfo.CancelAllowed) {
         getTLOptionValue("TLBotCancellationPolicy", action);
      }
      else
      {
         ChatHandler(ActiveCustomerInfo, 'left cancelaccount');
         setTimeout(function() {
            ClientConversation('ChooseOptionFreezeCancel');
         }, 2000);
      }
   }

   if(action == 'AllowFreeze')
   {
      getTLOptionValue("TLBotOfferFreezeTerms", action);
   }
}

function EnableButton()
{
   var firtname = jQuery("#firtname").val();
   var lastname  = jQuery("#lastname").val();
   var phonenumber  = jQuery("#phonenumber").val();

   if(firtname != '' && lastname != '' && phonenumber != '')
   {
      jQuery('#submit').removeAttr('disabled');
   }
   else
   {
      jQuery('#submit').attr('disabled', 'disabled');
   }
}

// Verify Customer
function PerformTask(response) {
   jQuery('#loading').show();
   var fname = jQuery("#firtname").val();  
   var lname = jQuery("#lastname").val();  
   var GetFirstLastName = fname+' '+lname;
   var phonenumber = jQuery("#phonenumber").val();
   jQuery("#firtname, #lastname, #phonenumber, #submit").attr('disabled', 'disabled');

   jQuery.ajax({
      method: "GET",
      headers: {
         "content-type": "application/x-www-form-urlencoded"
      },
      data: {
         "Action": "api",
         "fn": "getCustomerInfo",
         "name": GetFirstLastName,
         "phone": phonenumber,
         "type": "TLBot"
      },
      success: function(response) {
         var response = JSON.parse(response);
         ActiveCustomerInfo = response;
         var url = new URL(jQuery('#script').attr('src'));
         var ClientURL = url.searchParams.get("ClientURL");
         ActiveCustomerInfo.OnlinePaymentLink = "<a href='"+ClientURL+response.OnlinePaymentLink+"'  target='_blank'>"+ ClientURL+response.OnlinePaymentLink +"</a>";
         if(response.Status == 0) 
         {
            jQuery('#loading').hide();
            ClientConversation('AccountNotFound');
         } 
         else 
         {
            sendCustomerVerifyText(fname, lname, phonenumber, response.DateJoined);
         }  
      }
  });
}

//Send Verification Code to Customer
function sendCustomerVerifyText(fname, lname, phonenumber, DateJoined) {
   jQuery('#loading').show();
   var name = fname+' '+lname;

   jQuery.ajax({
      method: "GET",
      headers: {
         "content-type": "application/x-www-form-urlencoded"
      },
      data: {
         "Action": "api",
         "fn": "sendVerifyText",
         "name": name,
         "phone": phonenumber
      },
      success: function(response) {
         var response = JSON.parse(response);
         if(response.Status == 0) 
         {
            ClientConversation('InvalidPhoneNumber');
         } 
         else 
         {
            jQuery('#loading').hide();
            ClientConversation('VerificationCodeSent');
            jQuery('#loading').show();
            setTimeout(function() {
               jQuery('#loading').hide();
               jQuery('#chat').append("<div class='message right verifyphone'><div class='row'><div class='col-xs-6'></div><div class='col-xs-6'><input type='number' class='form-control' id='code' placeholder='Code'  onchange='ConfirmPhoneNumber(this, \""+name+"\", \""+phonenumber+"\");' onpaste='this.onchange();'></div></div></div>");
            }, 2500);  
            scrollToBottom();
         }
      }
  }); 
}

// Confirm Phone Number
function ConfirmPhoneNumber(event, name, phonenumber) {
   var code = event.value;
   jQuery(event).attr('disabled', 'disabled');
   var DateJoined = ActiveCustomerInfo.DateJoined;
   var length = code.toString().length;
   if(length >= 4) 
   {
      jQuery('#loading').show();
      jQuery.ajax({
         method: "GET",
         headers: {
            "content-type": "application/x-www-form-urlencoded"
         },
         data: {
            "Action": "api",
            "fn": "confirmVerifyText",
            "code": code,
            "phone": phonenumber
         },
         success: function(response) {
           var response = JSON.parse(response);
           jQuery('#loading').hide();
           if(response.Status == 0) {
               jQuery(".customerinfo, .TextCodeNotVerified").remove();
               ClientConversation('TextCodeNotVerified'); 
           } else {
               response.Message = "Thank you "+ ActiveCustomerInfo.FirstName+' '+ActiveCustomerInfo.LastName +".<br><br>You have been with us since "+ ActiveCustomerInfo.DateJoined +", we really appreciate your business.<br><br> Please choose an option below.";
               ChatHandler(response, "left");
               if(ActiveCustomerInfo.PastDueAmount > 0) 
               {
                  ClientConversation('PayBalance', ActiveCustomerInfo.OnlinePaymentLink); 
               } 
               else 
               {
                  ClientConversation('ChooseOptionFreezeCancel'); 
                  return false;
               }
            }
            scrollToBottom();
         }
      });
   }
}

// Get Cancel or Freeze Account Policy
function getTLOptionValue(OptionName, action = '') {
   jQuery.ajax({
      method: "GET",
      headers: {
         "content-type": "application/x-www-form-urlencoded"
      },
      data: {
         "Action": "api",
         "fn": "getOpt",
         "optionname": OptionName
      },
      success: function(response) {
         var result = {};
         if(action == 'header')
         {
            BusinessName = response;
            jQuery('.chatbox-popup__header h2').text(BusinessName);
         }
         else
         {
            result.Message = response;
            ChatHandler(result, 'left '+OptionName);
            if(OptionName == 'TLBotOfferFreezeTerms') 
            {
               ClientConversation('GetFreezeMessage');
               setTimeout(function() {
                  result.Message = "<div class='row'><div class='col-xs-6'></div><div class='col-xs-6'><input type='number' class='form-control' id='numberofmonths' placeholder='Enter Months' onchange='"+action+"(this)'></div></div>";
                  ChatHandler(result, 'right freezeaccount');
               }, 2000);
            } 
            else if(OptionName == 'TLBotCancellationPolicy') 
            {
               ClientConversation('GetCancellationMessage');
               setTimeout(function() {
                  result.Message = "<div class='row'><div class='col-xs-4'></div><div class='col-xs-8'><input type='text' class='form-control cancelReason' id='cancelReason' placeholder='Enter Reason to Cancel' onchange='"+action+"(this)'></div></div>";
                  ChatHandler(result, 'right cancelaccount');
               }, 2000);
               
            } 
            scrollToBottom();
         }
      }
   });
}

// Cancel Customer Account
function CancelAccount(event) {
   var cancelreason = event.value;
   jQuery(event).attr('disabled', 'disabled');
   jQuery.ajax({
      method: "GET",
      headers: {
         "content-type": "application/x-www-form-urlencoded"
      },
      data: {
         "Action": "api",
         "fn": "cancelAccount",
         "cancelreason": cancelreason,
         "accountid": ActiveCustomerInfo.AccountID
      },
      success: function(response) {
         var response = JSON.parse(response);
         if(response.Status == 0) 
         {
            ChatHandler(response, 'left customer-error');
            ClientConversation('ChooseOptionFreezeCancel');
         } 
         else
         {
            getTLOptionValue("TLBotNotifyCustomerOfCancelMessage");
            setTimeout(function() {
               getTLOptionValue('TLBotEndOfChatMessage');
               ClientConversation('ChooseOptionFreezeCancel');
            }, 2000);
         } 
      }
   });
}

// Allow Freeze
function AllowFreeze(event) {
   jQuery('#loading').show();
   jQuery(event).attr('disabled', 'disabled');
   var numberofmonths = event.value;
   var phonenumber = jQuery("#phonenumber").val();
   
   jQuery.ajax({
      method: "GET",
      headers: {
         "content-type": "application/x-www-form-urlencoded"
      },
      data: {
         "Action": "api",
         "fn": "allowFreeze",
         "accountid": ActiveCustomerInfo.AccountID,
         "numberofmonths": numberofmonths
      },
      success: function(response) {
         var response = JSON.parse(response);
         if(response.Status == 0) 
         {
            jQuery('#loading').hide();
            ChatHandler(response, 'left customer-error');
            setTimeout(function() {
               ClientConversation('ChooseOptionFreezeCancel');
            }, 2000);
         } 
         else
         {
            FreezeAccount(ActiveCustomerInfo.AccountID, phonenumber, numberofmonths)
         } 
      }
   });
}

// Freeze Customer Account
function FreezeAccount(AccountID, phonenumber, numberofmonths) {

    jQuery.ajax({
        method: "GET",
        headers: {
            "content-type": "application/x-www-form-urlencoded"
        },
        data: {
            "Action": "api",
            "fn": "freezeAccount",
            "accountid": AccountID,
            "phonenumber": phonenumber,
            "numberofmonths": numberofmonths,
            "notifyonfreeze": 1
        },
        success: function(response) {
            jQuery('#loading').hide();
            var response = JSON.parse(response);
            if(response.Status == 0) 
            {
                ChatHandler(response, 'left customer-error');
            } 
            else
            {
                ChatHandler(response, 'left accountFrozen');
                setTimeout(function() {
                    getTLOptionValue('TLBotEndOfChatMessage');
                    ClientConversation('ChooseOptionFreezeCancel');
                }, 2000);
            } 
        }
    });
};