﻿using System;
using Application.Interfaces.Trade;
using Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Extensions;
using RabbitMQ.TradeGateway.Setup;
using RabbitMQ.TradeGateway.Util;

namespace RabbitMQ.TradeGateway
{
    public class Publisher :ITradeInform, ITrade, IDisposable
    {
        private readonly ConnectionWrapper _connectionWrapper;
        private string Author { get; }

        private IModel _channel;

        public Publisher(ConnectionWrapper connectionWrapper, RabbitMQInitializer iniInitializer)
        {
            _connectionWrapper = connectionWrapper;
            Author = iniInitializer.Author;
            while (!iniInitializer.IsInitialized);
        }

        public void Inform(TradeInformation trade)
        {
            Publish(trade,Exchange.Info,Author);
        }

        public void Buy(BuyOffer offer)
        {
            Publish(offer, Exchange.Buy, Author);
        }

        public void Sell(SellOffer offer)
        {
            Publish(offer, Exchange.Sell, Author);
        }

        private void Publish(TradeBase trade,string exchange,string author)
        {
            if (_channel == null || _channel.IsClosed)
            {
                _channel = _connectionWrapper.NewChannel();
            }
            _channel.BasicPublish(exchange, trade, $"{trade.Category}.{trade.ItemName}.{author}");

        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}