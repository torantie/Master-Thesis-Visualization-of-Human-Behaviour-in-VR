import json


class SentenceSentiment:

    def __init__(self, sentence, sentiment):
        self.sentence = sentence
        self.sentiment = sentiment

    def to_json(self):
        return json.dumps(self, default=lambda o: o.__dict__)
