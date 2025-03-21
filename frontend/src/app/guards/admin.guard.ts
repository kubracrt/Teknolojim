import { Router, CanActivateFn} from '@angular/router';
import { inject } from '@angular/core';

export const adminGuard: CanActivateFn = () => {
  const token = localStorage.getItem("token");
  const router = inject(Router);

  if (token) {
    return true;
  } else {
    router.navigate(["/login"])
    return false;
  }
}

