#! /usr/bin/python

import math
import numpy as np

class MarketEnv:
    def __init__(self):        
        self.trainingWindow = 60
        self.slippage = 0.001
        self.pv = 1.0
        self.shape = (-1, 50, 20, 1)

    def initialize(self, filePath):
        # read data from filePath
        self.indexData = np.loadtxt(filePath + '\\index.txt')
        self.folder = filePath

    def getPV(self):
        return self.pv

    """
    action: 1 - buy, 2 - sell, 3 - nothing
    """
    def nextTradingCycle(self, action, useCNN = False):        
        reward = 0
        executionPrice = self.prices[self.cursor + 1][3] #next open price

        if action == 1: 
            # buy
            if self.buyPrice == 0:
                self.buyPrice = executionPrice* (1 + self.slippage)
        elif action == 2:
            # sell
            if self.buyPrice > 0:                
                reward = executionPrice / self.buyPrice - 1.0
                self.pv = self.pv * (executionPrice / self.buyPrice)
                self.buyPrice = 0
        else:
            pass
        
        self.cursor += 1
        done = (self.cursor >= self.trainingWindow - 1)
        nextCycle = self.trainingCycles[self.cursor].reshape(self.shape) if useCNN else self.trainingCycles[self.cursor]
        return nextCycle, reward, done

    """
    randomly pick up a trading period and return the first trading cycle
    """
    def reset(self, useCNN = False):
        while True:
            self.categary = math.ceil(np.random.rand() * len(self.indexData))
            situationSeries = np.loadtxt(self.folder + '\\' + str(self.categary) + '_charts.txt')
            priceSeries = np.loadtxt(self.folder + '\\' + str(self.categary) + '_prices.txt')
            startPosition = math.ceil(np.random.rand() * (len(situationSeries) - self.trainingWindow - 1))
            #print("trying categary %d and start with position %d." % (self.categary, startPosition))
            if startPosition > 0:
                self.trainingCycles = situationSeries[startPosition : startPosition + self.trainingWindow]
                self.prices = priceSeries[startPosition : startPosition + self.trainingWindow + 1]
                self.cursor = 0
                self.buyPrice = 0
                self.pv = 1.0
                print("Picked up categary %d and start with position %d." % (self.categary, startPosition))
                
                return self.trainingCycles[self.cursor].reshape(self.shape) if useCNN else self.trainingCycles[self.cursor]


