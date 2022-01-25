from germansentiment import SentimentModel

from SentenceSentiment import SentenceSentiment
from Sentiment import Sentiment


class SentimentAnalysis:
    def __init__(self):
        self.model = SentimentModel()

    def get_sentence_sentiments(self, sentences):
        self.model = SentimentModel()
        sentiment_strings = self.model.predict_sentiment(sentences)
        sentence_sentiments = []

        for i, sentence in enumerate(sentences):
            sentiment = Sentiment[sentiment_strings[i]].value
            sentence = sentences[i]
            sentence_sentiment = SentenceSentiment(sentence, sentiment).__dict__
            sentence_sentiments.append(sentence_sentiment)

        print("get_sentence_sentiments: " + str(sentence_sentiments))
        return sentence_sentiments
