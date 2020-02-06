using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Logging;

using CartService.Models;
using PsmPromoEvent = PricingServiceModel.PromoEvent; 
using PsmShoppingCart = PricingServiceModel.ShoppingCart;
using PsmShoppingCartItem = PricingServiceModel.ShoppingCartItem;

namespace CartService.Services {

    public class BatchExecutionCommand {
        public BatchExecutionCommand(object obj = null) {
            throw new Exception("BatchExecutionCommand - Temporary object written to enable the code to build. Replace before run time.");
        }
    }

    public class PromoEvent {
        public PromoEvent(object obj, object obj2) {
            throw new Exception("PromoEvent - Temporary object written to enable the code to build. Replace before run time.");
        }
    }

    public class ShoppingCartServiceImplDecisionServer: ShoppingCartServiceImpl {

        ILogger<ShoppingCartServiceImplDecisionServer> log;

        private static string CATALOG_ENDPOINT = Environment.GetEnvironmentVariable("CATALOG_ENDPOINT");
        private static string PRICING_ENDPOINT = Environment.GetEnvironmentVariable("PRICING_ENDPOINT");
        private static string URL = PRICING_ENDPOINT + "/kie-server/services/rest/server";
        private static string USER = Environment.GetEnvironmentVariable("KIE_SERVER_USER");
        private static string PASSWORD = Environment.GetEnvironmentVariable("KIE_SERVER_PASSWORD");
        private static string CONTAINER_SPEC = Environment.GetEnvironmentVariable("KIE_CONTAINER_DEPLOYMENT");
        private static string CONTAINER_ID = CONTAINER_SPEC.Substring(0, CONTAINER_SPEC.IndexOf('='));
        private static string KIE_SESSION_NAME = "coolstore-kie-session";
        private static string RULEFLOW_PROCESS_NAME = "com.redhat.coolstore.PriceProcess";

        // private static final MarshallingFormat FORMAT = MarshallingFormat.XSTREAM;

        // private KieServicesConfiguration conf;
        // private KieServicesClient kieServicesClient;
        // private RuleServicesClient rulesClient;

        public ShoppingCartServiceImplDecisionServer(ILogger<ShoppingCartServiceImplDecisionServer> logger) : base(logger) {
            log = logger;
        }

        /**
        * Initializes the KIE-Server-Client.
        */
        private void Initialize() {

            log.LogInformation("Initializing DecisionServer client.");
            // conf = KieServicesFactory.newRestConfiguration(URL, USER, PASSWORD);
            // conf.setMarshallingFormat(FORMAT);
            // kieServicesClient = KieServicesFactory.newKieServicesClient(conf);
            // rulesClient = kieServicesClient.getServicesClient(RuleServicesClient.class);

            string host = Environment.GetEnvironmentVariable("DATAGRID_HOST");
            string port = Environment.GetEnvironmentVariable("DATAGRID_PORT");
        }

        public override void PriceShoppingCart(ShoppingCart sc) {
            if (sc != null) {

                InitShoppingCartForPricing(sc);

                BatchExecutionCommand batchCommand = BuildBatchExecutionCommand(sc);

                // ServiceResponse<ExecutionResults> executeResponse = rulesClient.executeCommandsWithResults(CONTAINER_ID, batchCommand);

                // if (executeResponse.getType() == ResponseType.SUCCESS) {
                //     ExecutionResults results = executeResponse.getResult();
                //     PsmShoppingCart resultSc = (PsMShoppingCart) results.getValue("shoppingcart");
                //     MapShoppingCartPricingResults(resultSc, sc);
                // } else {
                    // TODO: Some proper, micro-service type error handling here.
                    string message = "Error calculating prices.";
                    log.LogError(message);
                    throw new Exception(message);
                // }
            }
        }

        /**
        * Builds the KIE {@link BatchExecutionCommand}, which contains all the KIE logic like insertion of facts, starting of ruleflow
        * processes and firing of rules, from the given {@link ShoppingCart}.
        *
        * @param sc the {@link ShoppingCart} from which the build the {@link BatchExecutionCommand}.
        * @return the {@link BatchExecutionCommand}
        */
        private BatchExecutionCommand BuildBatchExecutionCommand(ShoppingCart sc) {
            // KieCommands commandsFactory = KieServices.Factory.get().getCommands();
            // List of BRMS commands that will be send to the rules-engine (e.g. inserts, fireAllRules, etc).
            // IList<Command<object>> commands = new List<object>();

            // Insert the promo first. Promotions are retrieved from the PromoService.
            foreach(var promo in ps.Promotions) {
                PsmPromoEvent promoEvent = new PsmPromoEvent(promo.ItemId, promo.PercentOff);
                // Note that we insert the fact into the "Promo Stream".
                // Command<type> insertPromoEventCommand = commandsFactory.newInsert(promoEvent, "outPromo", false, "Promo Stream");
                // commands.add(insertPromoEventCommand);
            }

            /*
            * Build the ShoppingCart fact from the given ShoppingCart.
            */
            PsmShoppingCart factSc = BuildShoppingCartFact(sc);

            // commands.add(commandsFactory.newInsert(factSc, "shoppingcart", true, "DEFAULT"));

            // Insert the ShoppingCartItems.
            IList<ShoppingCartItem> scItems = sc.ShoppingCartItemList;
            foreach(ShoppingCartItem nextSci in scItems) {
                // Build the ShoppingCartItem fact from the given ShoppingCartItem.
                PsmShoppingCartItem factSci = BuildShoppingCartItem(nextSci);
                factSci.ShoppingCart = factSc;
                // commands.add(commandsFactory.newInsert(factSci));
            }

            // Add extra fireAllRules command. Workaround for: https://issues.jboss.org/browse/DROOLS-1593
            // TODO: Remove workaround when bug has been fixed.
            // commands.add(commandsFactory.newFireAllRules());

            // Start the process (ruleflow).
            // commands.add(commandsFactory.newStartProcess(RULEFLOW_PROCESS_NAME));

            // Fire the rules
            // commands.add(commandsFactory.newFireAllRules());

            BatchExecutionCommand batchCommand = new BatchExecutionCommand(); // = commandsFactory.newBatchExecution(commands, KIE_SESSION_NAME);
            return batchCommand;
        }

        /**
        * Builds a {@link PricingServiceModel.ShoppingCart} fact from the given {@link ShoppingCart}.
        *
        * @param sc the {@link ShoppingCart} from which to build the fact.
        * @return the {@link PricingServiceModel.ShoppingCart} fact
        */
        private PsmShoppingCart BuildShoppingCartFact(ShoppingCart sc) {
            PsmShoppingCart factSc = new PsmShoppingCart();
            factSc.CartItemPromoSavings = sc.CartItemPromoSavings;
            factSc.CartItemTotal = sc.CartItemTotal;
            factSc.CartTotal = sc.CartTotal;
            factSc.ShippingPromoSavings = sc.ShippingPromoSavings;
            factSc.ShippingTotal = sc.ShippingTotal;
            return factSc;
        }

        /**
        * Builds a {@link PricingServiceModel.ShoppingCartItem} fact from the given {@link ShoppingCartItem}.
        *
        * @param sci the {@link ShoppingCartItem} from which to build the fact.
        * @return the {@link PricingServiceModel.ShoppingCartItem} fact.
        */
        private PsmShoppingCartItem BuildShoppingCartItem(ShoppingCartItem sci) {
            PsmShoppingCartItem factSci = new PsmShoppingCartItem();
            factSci.ItemId = sci.Product.ItemId;
            factSci.Name = sci.Product.Name;
            factSci.Price = sci.Product.Price;
            factSci.Quantity = sci.Quantity;
            return factSci;
        }

        /**
        * Maps the {@link PricingServiceModel.ShoppingCart} pricing results to the given {@link ShoppingCart}.
        *
        * @param resultSc the {@link PricingServiceModel.ShoppingCart} containing the pricing defined by the rules engine.
        * @param sc       the {@link ShoppingCart} onto which we need to map the results.
        */
        private void MapShoppingCartPricingResults(PsmShoppingCart resultSc, ShoppingCart sc) {
            sc.CartItemPromoSavings = resultSc.CartItemPromoSavings;
            sc.CartItemTotal = resultSc.CartItemTotal;
            sc.ShippingPromoSavings = resultSc.ShippingPromoSavings;
            sc.ShippingTotal = resultSc.ShippingTotal;
            sc.CartTotal = resultSc.CartTotal;
        }
    }

}