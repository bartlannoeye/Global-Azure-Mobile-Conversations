// credits to https://github.com/cobey/XamarinShoppingBotSample for the dialog
// LUIS json at https://gist.github.com/anonymous/6ac446635e316ec8399f66198b4be490
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace ShoppingBot.Dialogs
{
    //[LuisModel("{application guid}", "{key guid}")]
    [LuisModel("ffffffff-4600-41ee-bdbb-000000000000", "fffffffff7ad4b52a928000000000000")] // fake keys ofc
    [Serializable]
    public class ShoppingDialog : LuisDialog<object>
    {
        private readonly List<string> _customerSupportKeywords =
            new List<string> {"password", "cancel", "login", "log", "create"};

        [LuisIntent("")]
        public async Task CatchAll(IDialogContext context, LuisResult result)
        {
            string message =
                $"Sorry, I do not understand: {string.Join(", ", result.Intents.Select(i => i.Intent))}. Try asking me for order information or customer service help";

            await context.PostAsync(message);
            context.Done(true);
        }

        [LuisIntent("OrderStatus")]
        public async Task OrderStatus(IDialogContext context, LuisResult result)

        {
            var entities = result.Entities;

            bool hasOrderNumber = false;

            int orderNumber = 0;

            foreach (var entity in entities)
            {
                if (entity.Type == "OrderNumber")
                {
                    if (int.TryParse(entity.Entity, out orderNumber))
                    {
                        hasOrderNumber = true;
                    }
                    else
                    {
                        PromptDialog.Text(context, OrderNumberAdded, "Please enter a valid order number");
                    }
                }
            }

            if (hasOrderNumber)
            {
                if (Convert.ToInt32(orderNumber) % 2 == 0)
                {
                    await context.PostAsync(
                        $"It looks like order {orderNumber} will be delivered on {DateTime.Now.AddDays(2).ToShortDateString()}");

                }
                else
                {
                    await context.PostAsync($"Hooray! It looks like {orderNumber} is scheduled for delivery today!");
                }
            }
            else
            {
                PromptDialog.Text(context, OrderNumberAdded, "Please enter a valid order number");
            }
        }

        private async Task OrderNumberAdded(IDialogContext context, IAwaitable<string> result)
        {
            var orderNumber = await result;
            var deliveryDate = checkOrderStatus(Convert.ToInt32(orderNumber));

            if (deliveryDate.Date == DateTime.Now.Date)
            {
                await context.PostAsync("It looks like your order will be delivered today!");
                context.Done(result);
            }
            else
            {
                await context.PostAsync("Your order is scheduled for delivery on " + deliveryDate.ToLongDateString());
                context.Done(result);
            }
        }

        private DateTime checkOrderStatus(int orderNumber)
        {
            if (Convert.ToInt32(orderNumber) % 2 == 0)
            {
                return DateTime.Now.AddDays(2);
            }
            else
            {
                return DateTime.Now;
            }
        }

        [LuisIntent("CustomerService")]
        public async Task CustomerServiceRequest(IDialogContext context, LuisResult result)
        {
            foreach (var entity in result.Entities)
            {
                if (entity.Type == "ServiceKeyword" && _customerSupportKeywords.Contains(entity.Entity.ToLower()))
                {
                    switch (entity.Entity.ToLower())
                    {
                        case "password":
                            PromptDialog.Text(context, SupportUsernameEntered,
                                "What is the email address associated with your account?");
                            break;

                        case "cancel":
                            PromptDialog.Text(context, SupportCancelOrderEntered, "Please enter order information");
                            break;

                        case "login":
                            PromptDialog.Text(context, SupportLoginError,
                                "Please provide the username that is associated with your account");
                            break;

                        case "log":
                            PromptDialog.Text(context, SupportLoginError,
                                "Please provide the username that is associated with your account");
                            break;

                        case "sign":
                            await context.PostAsync("You can sign up for an account at https://xamarin.com");
                            context.Done(true);
                            break;

                        case "create":
                            await context.PostAsync("You can create an account at https://xamarin.com");
                            context.Done(true);
                            break;
                    }
                }
            }
        }

        private async Task SupportLoginError(IDialogContext context, IAwaitable<string> result)
        {
            var item = await result;

            PromptDialog.Choice(context, ContactMethodSelected, new string[] {"Phone", "SMS"},
                "We need to verify your account, please select a contact method");
        }

        private async Task ContactMethodSelected(IDialogContext context, IAwaitable<string> result)
        {
            var method = await result;
            await context.PostAsync($"We will contact you via {method} to verify your login account");
        }

        private Task SupportCancelOrderEntered(IDialogContext context, IAwaitable<string> result)
        {
            throw new NotImplementedException();
        }

        private async Task SupportUsernameEntered(IDialogContext context, IAwaitable<string> result)
        {
            var item = await result;
            {
                await context.PostAsync("Thanks! We have sent a reset link to your email address!");
                context.Done(true);
            }
        }

        private async Task AfterDescriptionIssues(IDialogContext context, IAwaitable<string> result)
        {
            string userIssue = await result; // do nothing with it
            PromptDialog.Choice(context, AfterUserHasChosenAsync, new string[] {"Call", "Text", "SMS"},
                "How would you like to be contacted?");
        }

        private async Task AfterUserHasChosenAsync(IDialogContext context, IAwaitable<string> result)
        {
            string userChoice = await result;

            switch (userChoice)
            {
                case "Call":
                    PromptDialog.Number(context, UserProvidesNumber, "What is your phone number?",
                        "Please enter a valid phone number", 3);
                    break;

                case "Text":
                    await context.PostAsync("Let's start a chat session!");
                    break;

                case "SMS":
                    await context.PostAsync("Please text us at 503-555-5555");
                    break;

            }
        }

        private async Task UserProvidesNumber(IDialogContext context, IAwaitable<long> result)
        {
            long phoneNumber = await result;

            await context.PostAsync("We will call you at " + phoneNumber);
            context.Done(phoneNumber);
        }
    }
}