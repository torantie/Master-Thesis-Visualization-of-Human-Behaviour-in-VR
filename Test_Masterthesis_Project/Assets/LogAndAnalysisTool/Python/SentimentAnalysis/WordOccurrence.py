import json


class WordOccurrence:

    def __init__(self, word, occurrence):
        self.word = word
        self.occurrence = occurrence

    def to_json(self):
        return json.dumps(self, default=lambda o: o.__dict__)
