/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { Component } from '@angular/core';

import { AuthService } from '@app/shared';

@Component({
    selector: 'sqx-home-page',
    styleUrls: ['./home-page.component.scss'],
    templateUrl: './home-page.component.html'
})
export class HomePageComponent {
    public showLoginError = false;

    constructor(
        private readonly authService: AuthService
    ) {
    }

    public login() {

        this.authService.loginRedirect();
        }

}