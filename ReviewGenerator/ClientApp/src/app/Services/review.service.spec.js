"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
require("jasmine");
var review_service_1 = require("./review.service");
describe('ReviewService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(review_service_1.ReviewService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=review.service.spec.js.map