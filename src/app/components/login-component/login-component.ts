import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from "@angular/router";
import { LoginRequest } from '../../core/models/auth';
import { AuthService } from '../../core/services/auth-service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login-component',
  imports: [RouterLink, FormsModule, CommonModule],
  templateUrl: './login-component.html',
  styleUrl: './login-component.css',
})
export class LoginComponent {
  email: string;
  password: string;
  errMsg: string | null;
  isSubmitting: boolean;
  seePassword: boolean;
  
  constructor(private authService: AuthService, private router: Router){
    this.email = "";
    this.password = "";
    this.errMsg = "";
    this.isSubmitting = false;
    this.seePassword = false;
  }

  showPassword(){
    this.seePassword = !this.seePassword;
  }

  onSubmit(): void {
  if (!this.email || !this.password) {
    this.errMsg = '*Email or Password cannot be empty.';
    this.isSubmitting = false;
    return;  // ← also add return here so it doesn't continue
  }

  const data: LoginRequest = {
    email: this.email,
    password: this.password
  };

  this.authService.login(data).subscribe({
    next: () => {
      this.isSubmitting = true;
      this.errMsg = null;
      this.redirectByRole();  // ← call this instead of router.navigate directly
    },
    error: (err: any) => {
      this.isSubmitting = false;
      this.errMsg = '*Invalid email or password';
      console.error(err);
    }
  });
}

private redirectByRole(): void {
  const role = this.authService.getUserRole();
  const roleRoutes: Record<string, string> = {
    Citizen:             '/citizen-home',
    Doctor:              '/home',
    LabTechnician:       '/home',
    PublicHealthOfficer: '/home',
    Researcher:          '/home',
    Admin:               '/home/admin',
    ComplianceOfficer:   '/compliance'
  };
  this.router.navigate([roleRoutes[role ?? ''] ?? '/home']);
}

}
